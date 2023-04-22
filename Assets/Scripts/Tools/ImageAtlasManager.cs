using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

/**
* 图集管理
*/
public class ImageAtlasManager : Singleton<ImageAtlasManager>
{

    public Material GreyShaderMat;

    //系统性 常开
    public SpriteAtlas WaterPipeAtlas;

    

    public void LoadAtlas()
    {
        WaterPipeAtlas = Resources.LoadAsync<SpriteAtlas>("AtlasPack/WaterPipe").asset as SpriteAtlas;

    }





}
