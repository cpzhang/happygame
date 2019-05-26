using ILRuntime.CLR.Method;
using ILRuntime.CLR.TypeSystem;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.Runtime.Stack;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Bootstrapper : MonoBehaviour
{
    ILRuntime.Runtime.Enviorment.AppDomain appdomain;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("bootstrapper start");
        _ = StartCoroutine(LoadILRuntime());
    }

    IEnumerator LoadILRuntime()
    {
        appdomain = new ILRuntime.Runtime.Enviorment.AppDomain();
#if UNITY_ANDROID
        WWW www = new WWW(Application.streamingAssetsPath + "/HotFix.dll");
#else
        WWW www = new WWW("file:///" + Application.streamingAssetsPath + "/HotFix.dll");
#endif
        while (!www.isDone)
            yield return null;
        if (!string.IsNullOrEmpty(www.error))
            Debug.LogError(www.error);
        byte[] dll = www.bytes;
        www.Dispose();
#if UNITY_ANDROID
        www = new WWW(Application.streamingAssetsPath + "/HotFix.pdb");
#else
        www = new WWW("file:///" + Application.streamingAssetsPath + "/HotFix.pdb");
#endif
        while (!www.isDone)
            yield return null;
        if (!string.IsNullOrEmpty(www.error))
            Debug.LogError(www.error);
        byte[] pdb = www.bytes;
        using (System.IO.MemoryStream fs = new MemoryStream(dll))
        {
            using (System.IO.MemoryStream p = new MemoryStream(pdb))
            {
                appdomain.LoadAssembly(fs, p, new ILRuntime.Mono.Cecil.Pdb.PdbReaderProvider());
                OnILRuntimeInitialized();
                appdomain.Invoke("HotFix.Bootstrapper", "go", null, null);
            }
        }
    }
    private void OnILRuntimeInitialized()
    {
        appdomain.DebugService.StartDebugService(56000);
        InitializeILRuntime();
        SetupCLRRedirection();
        SetupCLRRedirection2();
    }
    unsafe void InitializeILRuntime()
    {
        //这里做一些ILRuntime的注册
        appdomain.RegisterCrossBindingAdaptor(new MonoBehaviourAdapter());
        appdomain.DelegateManager.RegisterMethodDelegate<string, string, LogType>();
        appdomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Application.LogCallback>((act) =>
        {
            return new UnityEngine.Application.LogCallback((condition, stackTrace, type) =>
            {
                ((Action<System.String, System.String, UnityEngine.LogType>)act)(condition, stackTrace, type);
            });
        });
        appdomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction>((act) =>
        {
            return new UnityEngine.Events.UnityAction(() =>
            {
                ((Action)act)();
            });
        });
        appdomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction<float>>((action) =>
        {
            return new UnityEngine.Events.UnityAction<float>((a) =>
            {
                ((System.Action<float>)action)(a);
            });
        });
    }
    unsafe void SetupCLRRedirection()
    {
        //这里面的通常应该写在InitializeILRuntime，这里为了演示写这里
        var arr = typeof(GameObject).GetMethods();
        foreach (var i in arr)
        {
            if (i.Name == "AddComponent" && i.GetGenericArguments().Length == 1)
            {
                appdomain.RegisterCLRMethodRedirection(i, AddComponent);
            }
        }
    }
    unsafe static StackObject* AddComponent(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
    {
        //CLR重定向的说明请看相关文档和教程，这里不多做解释
        ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;

        var ptr = __esp - 1;
        //成员方法的第一个参数为this
        GameObject instance = StackObject.ToObject(ptr, __domain, __mStack) as GameObject;
        if (instance == null)
            throw new System.NullReferenceException();
        __intp.Free(ptr);

        var genericArgument = __method.GenericArguments;
        //AddComponent应该有且只有1个泛型参数
        if (genericArgument != null && genericArgument.Length == 1)
        {
            var type = genericArgument[0];
            object res;
            if (type is CLRType)
            {
                //Unity主工程的类不需要任何特殊处理，直接调用Unity接口
                res = instance.AddComponent(type.TypeForCLR);
            }
            else
            {
                //热更DLL内的类型比较麻烦。首先我们得自己手动创建实例
                var ilInstance = new ILTypeInstance(type as ILType, false);//手动创建实例是因为默认方式会new MonoBehaviour，这在Unity里不允许
                //接下来创建Adapter实例
                var clrInstance = instance.AddComponent<MonoBehaviourAdapter.Adaptor>();
                //unity创建的实例并没有热更DLL里面的实例，所以需要手动赋值
                clrInstance.ILInstance = ilInstance;
                clrInstance.AppDomain = __domain;
                //这个实例默认创建的CLRInstance不是通过AddComponent出来的有效实例，所以得手动替换
                ilInstance.CLRInstance = clrInstance;

                res = clrInstance.ILInstance;//交给ILRuntime的实例应该为ILInstance

                clrInstance.Awake();//因为Unity调用这个方法时还没准备好所以这里补调一次
            }

            return ILIntepreter.PushObject(ptr, __mStack, res);
        }

        return __esp;
    }
    unsafe void SetupCLRRedirection2()
    {
        //这里面的通常应该写在InitializeILRuntime，这里为了演示写这里
        var arr = typeof(GameObject).GetMethods();
        foreach (var i in arr)
        {
            if (i.Name == "GetComponent" && i.GetGenericArguments().Length == 1)
            {
                appdomain.RegisterCLRMethodRedirection(i, GetComponent);
            }
        }
    }

    MonoBehaviourAdapter.Adaptor GetComponent(ILType type)
    {
        var arr = GetComponents<MonoBehaviourAdapter.Adaptor>();
        for (int i = 0; i < arr.Length; i++)
        {
            var instance = arr[i];
            if (instance.ILInstance != null && instance.ILInstance.Type == type)
            {
                return instance;
            }
        }
        return null;
    }
    unsafe static StackObject* GetComponent(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
    {
        //CLR重定向的说明请看相关文档和教程，这里不多做解释
        ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;

        var ptr = __esp - 1;
        //成员方法的第一个参数为this
        GameObject instance = StackObject.ToObject(ptr, __domain, __mStack) as GameObject;
        if (instance == null)
            throw new System.NullReferenceException();
        __intp.Free(ptr);

        var genericArgument = __method.GenericArguments;
        //AddComponent应该有且只有1个泛型参数
        if (genericArgument != null && genericArgument.Length == 1)
        {
            var type = genericArgument[0];
            object res = null;
            if (type is CLRType)
            {
                //Unity主工程的类不需要任何特殊处理，直接调用Unity接口
                res = instance.GetComponent(type.TypeForCLR);
            }
            else
            {
                //因为所有DLL里面的MonoBehaviour实际都是这个Component，所以我们只能全取出来遍历查找
                var clrInstances = instance.GetComponents<MonoBehaviourAdapter.Adaptor>();
                for (int i = 0; i < clrInstances.Length; i++)
                {
                    var clrInstance = clrInstances[i];
                    if (clrInstance.ILInstance != null)//ILInstance为null, 表示是无效的MonoBehaviour，要略过
                    {
                        if (clrInstance.ILInstance.Type == type)
                        {
                            res = clrInstance.ILInstance;//交给ILRuntime的实例应该为ILInstance
                            break;
                        }
                    }
                }
            }

            return ILIntepreter.PushObject(ptr, __mStack, res);
        }

        return __esp;
    }
}
