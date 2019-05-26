using UnityEngine;
using System.Collections;

public class UISmartWidget : MonoBehaviour {

    void Start()
    {
        Adjust();
    }

    void Adjust()
	{
		GameObject gameobject_ = gameObject;

        int beginDepth = 0;
        if (transform.parent != null)
        {
            UIWidget parentWidget = transform.parent.GetComponentInParent<UIWidget>();
            if (null != parentWidget)
            {
                beginDepth = parentWidget.depth;
            }
        }

		if(0 == beginDepth)
		{
			return;
		}
		
		UIWidget[] sonWidgets = gameobject_.GetComponentsInChildren<UIWidget>();
		foreach(UIWidget widget in sonWidgets)
        {
            widget.depth += beginDepth;
		}
	}
}
