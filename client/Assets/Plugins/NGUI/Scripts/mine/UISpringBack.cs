using UnityEngine;
using System.Collections;

public class UISpringBack : MonoBehaviour 
{
    TweenScale tween_scale_script_;
    EventDelegate finish_del_ = null;
    bool switch_ = false;
    bool next_ = false;
    bool finished_ = false;
    public int back_times_ = 0;

    void Awake()
    {
        switch_ = false;
        next_ = false;
        finished_ = false;
        finish_del_ = new EventDelegate(OnTweenFinish);
        tween_scale_script_ = gameObject.GetComponent<TweenScale>();
        tween_scale_script_.AddOnFinished(finish_del_);
    }

    void LateUpdate()
    {
        if (!finished_)
        {
            if (next_)
            {
                next_ = false;
                tween_scale_script_.AddOnFinished(finish_del_);
                tween_scale_script_.ResetToBeginning();
                tween_scale_script_.PlayForward();
            }
        }
    }

    void OnTweenFinish()
    {
        switch_ = !switch_;
        Vector3 from = Vector3.one;
        Vector3 to = Vector3.one;
        if (switch_)
        {
            back_times_--;
            from = tween_scale_script_.from;
            tween_scale_script_.from = tween_scale_script_.to;
            tween_scale_script_.to = tween_scale_script_.to + (from - tween_scale_script_.to) * 0.5f;
            from = tween_scale_script_.from;
            to = tween_scale_script_.to;
        }
        else
        {
            from = tween_scale_script_.from;
            tween_scale_script_.from = tween_scale_script_.to;
            tween_scale_script_.to = from;
            from = tween_scale_script_.from;
            to = tween_scale_script_.to;
        }
        if(back_times_ == 0 ||
            (Mathf.Abs(from.x - to.x) < 0.01f
            && Mathf.Abs(from.y - to.y) < 0.01f
            && Mathf.Abs(from.z - to.z) < 0.01f))
        {
            finished_ = true;
        }
        else
        {
            next_ = true;
        }
    }
}
