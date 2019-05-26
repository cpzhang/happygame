using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SmoothPage : MonoBehaviour
{
    class Item
    {
        public Transform trans_;
        public int length_;
    }
    Transform transform_;
    List<Item> pages_ = new List<Item>();
    UIScrollView scrollview_;
    float content_length_ = 0;
    int current_page_ = 0;
    float to_amount_ = 0f;
    bool moving_ = false;

    void Awake()
    {
        transform_ = transform;
    }

    void Start()
    {
        Collect();
    }

    void Update()
    {
        if (moving_)
        {
            UIScrollView sv = cacheScrollView();
            if (sv == null)
            {
                moving_ = false;
                return;
            }
            bool bh = (sv.movement == UIScrollView.Movement.Horizontal);
            float offset = Mathf.Abs(bh ? sv.panel.clipOffset.x : sv.panel.clipOffset.y);
            float curAmount = offset / content_length_;
            float a = curAmount + (to_amount_ - curAmount) * Time.deltaTime / 0.1f;
            if (Mathf.Abs(to_amount_ - a) < 0.01f)
            {
                sv.SetDragAmount((bh ? to_amount_ : 0.5f), (!bh ? to_amount_ : 0.5f), false);
                moving_ = false;
            }
            else
            {
                sv.SetDragAmount((bh ? a : 0.5f), (!bh ? a : 0.5f), false);
            }
        }
    }

    void Collect()
    {
        pages_.Clear();
        UIScrollView sv = cacheScrollView();
        if(sv == null)
        {
            return;
        }
        int childNum = transform_.childCount;
        bool bh = (sv.movement == UIScrollView.Movement.Horizontal);
        for (int i = 0; i < childNum;i++)
        {
            Item item = new Item();
            item.trans_ = transform_.GetChild(i);
            UIWidget wdg = item.trans_.GetComponent<UIWidget>();
            item.length_ = (int)(bh ? wdg.drawingDimensions.z - wdg.drawingDimensions.x : wdg.drawingDimensions.w - wdg.drawingDimensions.y);
            pages_.Add(item);
        }
        content_length_ = 468;// Mathf.Abs((bh ? sv.bounds.max.x + sv.bounds.min.x : sv.bounds.max.y + sv.bounds.min.y));
        current_page_ = 0;
        MovePage();
    }

    UIScrollView cacheScrollView()
    {
        if (scrollview_ == null)
        {
            scrollview_ = gameObject.GetComponentInParent<UIScrollView>();
        }
        return scrollview_;
    }

    public void PageDown()
    {
        current_page_++;
        MovePage();
    }

    public void PageUp()
    {
        current_page_--;
        MovePage();
    }

    public void FirstPage()
    {
        current_page_ = 0;
        MovePage();
    }

    void MovePage()
    {
        moving_ = true;
        int page = current_page_;
        int childNum = transform_.childCount;
        page = Mathf.Max(0,page);
        page = Mathf.Min(childNum - 1,page);
        int focus = 0;
        for (int i = 0; i < page;i++)
        {
            focus += pages_[i].length_;
        }
        to_amount_ = focus / content_length_;
    }
}
