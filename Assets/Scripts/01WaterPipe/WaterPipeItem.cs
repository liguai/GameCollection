using System;
using UnityEngine;
using UnityEngine.UI;
namespace WaterPipe
{
    
    /// <summary>
    /// 可以点击的水管组件
    /// </summary>
    public class WaterPipeItem : MonoBehaviour
    {
        public RectTransform rectTransform;
        public Image image;
        public Button button;
        
        private int rotate;
        private WaterPipeData data;

        private void Start()
        {
            button.onClick.AddListener(OnClickItem);
        }
        
        
        /// <summary>
        /// 更新位置
        /// </summary>
        /// <param name="_data"></param>
        /// <param name="size"></param>
        /// <param name="pos"></param>
        /// <param name="rotation"></param>
        public void InitData(WaterPipeData _data, Vector2 size, Vector2 pos , int rotation)
        {
            data = _data;
            image.sprite = ImageAtlasManager.Instance.WaterPipeAtlas.GetSprite(((int)_data.Enum).ToString());

            
            rotate = rotation;
            rectTransform.localPosition = pos;
            rectTransform.sizeDelta = size;
            rectTransform.localRotation = WaterPipeGameWin.GetRotationPos(rotation);
            OnEnableBtn();
            SetColor();
        }

        private void OnClickItem()
        {
            rotate += 1;
            rotate %= 4;
            if (rotate == 0)
            {
                rotate = 4;
            }
            rectTransform.localRotation = WaterPipeGameWin.GetRotationPos(rotate);
            WaterPipeGameWin.Instance.GetWaterPipeDataById(data.id).SetDir(rotate);
            
            //每次点击 就重新遍历路径是否通
            WaterPipeGameWin.Instance.CheackPath();
        }

        /// <summary>
        /// 设置是否可以点击
        /// </summary>
        /// <param name="istri"></param>
        public void OnEnableBtn(bool istri = true)
        {
            image.raycastTarget = istri;
        }

        /// <summary>
        /// 改变颜色
        /// </summary>
        public void SetColor(bool red = false)
        {
            if (red)
            {
                image.color = Color.red;
                return;
            }
            image.color = Color.white;
        }
    }


}