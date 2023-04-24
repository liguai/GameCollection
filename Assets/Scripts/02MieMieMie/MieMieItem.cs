using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



namespace MieMieMie
{
    
    public class MieMieItem : MonoBehaviour
    {
        public Button button;
        public Image image;
        public GameObject mask;
        public MieData data;
        
        private void Awake()
        {
            button = this.transform.GetComponent<Button>();
            mask = this.transform.Find("mask").gameObject;
            image = this.transform.GetComponent<Image>();
            button.onClick.AddListener(OnClickItem);
        }
        
        //初始化数据
        public void InitData(MieData _data)
        {
            data = _data;

            transform.position = MieMieMieGameWin.Instance.GetPositionByPos(data.posId);
            SerActiveMask(data.mask);
        }

        public void SerActiveMask(bool show = true)
        {
            mask.gameObject.SetActive(show);
            image.raycastTarget = !show;
        }

        public void SetRayCast(bool show = true)
        {
            image.raycastTarget = !show;

        }

        //点击了Item
        private void OnClickItem()
        {
            this.gameObject.SetActive(false);
            MieMieMieGameWin.Instance.RemoveItem(data.rid);
        }

    }
}
