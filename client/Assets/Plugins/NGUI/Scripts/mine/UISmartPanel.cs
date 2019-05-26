using UnityEngine;
using System.Collections;

public class UISmartPanel : MonoBehaviour {
	//是否设置为目前最大深度
	public bool is_top_ = false;
	//是否递归调整深度
	public bool is_recursive_ = true;
	//起始深度
	public int increase_depth_ = 0;

    void Start()
    {
        Adjust();
    }

    void Adjust()
    {
        GameObject gameobject_ = gameObject;

        int beginDepth = 0;
        if (is_top_)
        {
            beginDepth = UIPanel.nextUnusedDepth;
        }
        else
        {
            if (transform.parent != null)
            {
                UIPanel parentPanel = transform.parent.GetComponentInParent<UIPanel>();
                if (parentPanel != null)
                {
                    beginDepth = parentPanel.depth;
                }
            }
        }

        beginDepth += increase_depth_;
        if (0 == beginDepth)
        {
            return;
        }

        UIPanel panel = gameobject_.GetComponent<UIPanel>();
        if (null != panel)
        {
            panel.depth += beginDepth;
        }
        else
        {
            Debug.LogError("UISmartPanel should belong to the gamoObject contains UIPanel");
        }

        if (is_recursive_)
        {
            UIPanel[] sonPanels = gameobject_.GetComponentsInChildren<UIPanel>();
            foreach (UIPanel sonPanel in sonPanels)
            {
                if (sonPanel != panel)
                {
                    sonPanel.depth += beginDepth;
                }
            }
        }
    }
}
