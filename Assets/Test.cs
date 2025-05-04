using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    public Sprite m_OriginalSprite;
    public Image m_Image;
    private bool m_IsLoadTexture = false;
    private bool m_Resume = false;
    private FilterMode m_FilterMode = FilterMode.Bilinear;
    
    private void Awake()
    {
    }

    // Start is called before the first frame update
    void Start()
    {
        UpdateResolution();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_IsLoadTexture)
        {
            m_IsLoadTexture = false;
            BuildTexture();
        }
        if (m_Resume)
        {
            m_Resume = false;
            m_Image.sprite = m_OriginalSprite;
        }
    }
    
    private int[,] m_ResolutionArray = new int[,]
    {
        { 1920, 1080 },
        { 2560, 1440 }
    };
    
    private int m_ResolutionIndex = 0;
    
    
    public void OnGUI()
    {
        // GUIのスケール倍率調整
        GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(Screen.width / 1440f, Screen.height / 720f, 1f) * 3.0f);
        
        GUILayout.Label("Resolution: " + m_ResolutionArray[m_ResolutionIndex, 0] + "x" + m_ResolutionArray[m_ResolutionIndex, 1]);

        int resolutionIndex = GUILayout.SelectionGrid(m_ResolutionIndex, new string[] { "1920x1080", "2560x1440" }, 2);
        if (resolutionIndex != m_ResolutionIndex)
        {
            m_ResolutionIndex = resolutionIndex;
            UpdateResolution();
        }
        
        GUILayout.Label("FilterMode: " + m_FilterMode);
        m_FilterMode = (FilterMode)GUILayout.SelectionGrid((int)m_FilterMode, new string[] { "Point", "Bilinear", "Trilinear" }, 3);
        
        if(GUILayout.Button("Load Texture"))
        {
            m_IsLoadTexture = true;
        }
        if(GUILayout.Button("Resume"))
        {
            m_Resume = true;
        }
    }
    
    private void UpdateResolution()
    {
        Screen.SetResolution(m_ResolutionArray[m_ResolutionIndex, 0], m_ResolutionArray[m_ResolutionIndex, 1], FullScreenMode.Windowed);
    }
    
    private Texture2D m_LoadTextureTmp;
    private const int ByteSize = 1 * 1024 * 1024;
    private byte[] m_LoadTmpByteArray = new byte[ByteSize];
    
    private void BuildTexture()
    {
        Stream source = File.OpenRead(Application.streamingAssetsPath + "/Sample.png");
        // ReSharper disable once MustUseReturnValue
        source.Read(m_LoadTmpByteArray, 0, (int)source.Length);
        source.Close();
        m_LoadTextureTmp = new Texture2D(2, 2);
        m_LoadTextureTmp.LoadImage(m_LoadTmpByteArray);
        m_LoadTextureTmp.filterMode =  m_FilterMode;
        var rect = new Rect
        {
            width = m_LoadTextureTmp.width,
            height = m_LoadTextureTmp.height
        };
        var newSprite= Sprite.Create(
                    m_LoadTextureTmp, 
                    rect, 
                    new Vector2(m_OriginalSprite.pivot.x / m_OriginalSprite.rect.width, m_OriginalSprite.pivot.y / m_OriginalSprite.rect.height),
                    m_OriginalSprite.pixelsPerUnit,0, SpriteMeshType.FullRect);
        m_Image.sprite = newSprite;
    }
}
