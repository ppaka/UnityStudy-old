using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class ListItemManagerScript : MonoBehaviour
{
    public GameObject itemPrefab;

    [Header("Margin")]
    public bool autoMargin = true;
    public Vector2 inputMargin;

    [Header("Size")]
    public int itemCount = 100;

    GameObject targetContent;
    Vector2 viewSize,itemSize;
    Vector2 margin;
    int widthCount, heightCount;
    
    //myadd
    private int allCounts;

    DirectionStack<GameObject> myScrollContents;

    float topBound, botBound;
    int topId, botId;
    
    public void Init()
    {
        myScrollContents = new DirectionStack<GameObject>();
        targetContent = GetComponent<ScrollRect>().content.gameObject;

        viewSize = GetComponent<RectTransform>().sizeDelta;
        itemSize = itemPrefab.GetComponent<RectTransform>().sizeDelta;
        
        if (autoMargin)
        {
            
            widthCount = (int)(viewSize.x / itemSize.x);
            heightCount = (int)(viewSize.y / itemSize.y);

            // 아이템 사이 여백은 아이템 개수 + 1 개 존재한다.
            margin.x = (viewSize.x - itemSize.x * widthCount) / (float)(widthCount + 1) + itemSize.x;
            margin.y = (viewSize.y - itemSize.y * heightCount) / (float)(heightCount + 1) + itemSize.y;
        }
        else
        {
            widthCount = (int)(viewSize.x / itemSize.x);
            heightCount = (int)(viewSize.y / itemSize.y);
            margin = inputMargin;
        }

        Vector2 contentSize = targetContent.GetComponent<RectTransform>().sizeDelta;
        contentSize.y = margin.y * (Mathf.Ceil(itemCount / widthCount)) - itemSize.y; //+1 지움
        targetContent.GetComponent<RectTransform>().sizeDelta = contentSize;

        inputMargin = margin;

        int tempId = 0;

        // 세로 방향으로 위, 아래 (스크롤 감안) 2개 더 생성.
        for (int i = 0; i < heightCount + widthCount * 3; i++)
        {
            for (int j = 0; j < widthCount; j++)
            {
                GameObject contentObj = Instantiate(itemPrefab, targetContent.transform);
                Vector2 listPos = new Vector2(viewSize.x / 2 - widthCount / 2 * margin.x + margin.x * j, itemSize.y / 2 - margin.y * (i+1));
                contentObj.transform.localPosition = listPos;
                myScrollContents.PushBack(contentObj);
                contentObj.GetComponent<ListItemParentScript>().SetId(tempId);
                tempId++;
            }
        }

        topId = -1;
        botId = tempId - 1;
        allCounts += itemCount;

        topBound = margin.y/2;
        botBound = -viewSize.y - margin.y/2;
    }

    // Update is called once per frame
    void Update()
    {
        int intY = -(int)GetComponent<ScrollRect>().content.transform.localPosition.y;
        while (intY > topBound + margin.y / 2)
        {
            // 콘텐츠 위 부분 (아래로 스크롤)
            if(topId-1 < 0) break; // 맨 위면 풀링 금지

            topId--;
            botId--;

            GameObject botTarget = myScrollContents.PopBack();
            botTarget.GetComponent<ListItemParentScript>().SetId(topId);
            Vector2 tempPosition = new Vector2(viewSize.x / 2 - widthCount / 2 * margin.x + margin.x * (topId % widthCount), itemSize.y / 2 - margin.y * (topId / widthCount + 1));
            botTarget.transform.localPosition = tempPosition;
            myScrollContents.PushFront(botTarget);

            topBound += margin.y;
            botBound += margin.y;
        }
        
        while (intY - viewSize.y < botBound - margin.y / 2)
        {
            // 콘텐츠 아래 부분 (위로 스크롤)
            if(botId+1>allCounts) break; // 맨 아래면 풀링 금지

            topId++;
            botId++;

            GameObject botTarget = myScrollContents.PopFront();
            botTarget.GetComponent<ListItemParentScript>().SetId(botId);
            Vector2 tempPosition = new Vector2(viewSize.x / 2 - widthCount / 2 * margin.x + margin.x * (botId % widthCount), itemSize.y / 2 - margin.y * (botId / widthCount-1)); //-1 추가
            botTarget.transform.localPosition = tempPosition;
            myScrollContents.PushBack(botTarget);

            topBound -= margin.y;
            botBound -= margin.y;
        }
    }
}

// Copyright, jysa000@naver.com - 댄싱돌핀