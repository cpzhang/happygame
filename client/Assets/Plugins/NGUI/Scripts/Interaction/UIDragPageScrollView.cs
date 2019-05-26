using UnityEngine;
using System.Collections;

public class UIDragPageScrollView : MonoBehaviour
{

    UIScrollView scrollView;
    UIPanel uiPanel;

    int mStartPressID;
    int mEndPressID;
    Vector3 mStartPos;
    Vector3 mEndPos;
    Vector3 mCachedStartPos;
    public int mMinPage = 1;
    public int mMaxPage = 6;
    public int mCurrentPage = 1;
    public int mPageWidth = 432;
    public delegate void OnPageChange(int index);
    public OnPageChange onPageChange;
    public int mTriggerDistance = 50;
    // Use this for initialization
    void Awake()
    {
        scrollView = gameObject.GetComponent<UIScrollView>();
        uiPanel = gameObject.GetComponent<UIPanel>();
        mCachedStartPos = uiPanel.cachedGameObject.transform.localPosition;
        UIDragScrollView[] uiDragScrollViews = transform.parent.GetComponentsInChildren<UIDragScrollView>();
        for( int i =0; i < uiDragScrollViews.Length; ++i)
        {
            uiDragScrollViews[i].mPageControl = true;
        }

    }

    public void OnItemPressed( bool pressed)
    {

        if (pressed)
        {
            mStartPressID = UICamera.currentTouchID;
            mStartPos = UICamera.currentTouch.pos;
        }
        else
        {
            mEndPressID = UICamera.currentTouchID;
            mEndPos = UICamera.currentTouch.pos;
            if (mStartPressID == mEndPressID)
            {
                var diff = mEndPos - mStartPos;
                if (diff.x < -mTriggerDistance)
                {
                    if (mCurrentPage < mMaxPage)
                    {
                        mCurrentPage += 1;
                        SpringPanel.Begin(
                            uiPanel.cachedGameObject,
                            new Vector3(mCachedStartPos.x - (mCurrentPage -1)* mPageWidth,
                            uiPanel.cachedGameObject.transform.localPosition.y,
                            uiPanel.cachedGameObject.transform.localPosition.z),
                            8);

                    }

                }

                if (diff.x > mTriggerDistance)
                {
                    if (mCurrentPage > mMinPage)
                    {
                        mCurrentPage -= 1;
                        SpringPanel.Begin(
                            uiPanel.cachedGameObject,
                            new Vector3(mCachedStartPos.x - (mCurrentPage - 1) * mPageWidth,
                            uiPanel.cachedGameObject.transform.localPosition.y,
                            uiPanel.cachedGameObject.transform.localPosition.z),
                            8);

                    }

                }

                if (onPageChange != null)
                    onPageChange(mCurrentPage);
            }
        }
    }

}
