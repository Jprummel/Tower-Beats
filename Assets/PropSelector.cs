﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class PropSelector : MonoBehaviour
{
    public delegate void PropSelectorConfirm(string propPath);
    public static PropSelectorConfirm s_OnPropSelectorConfirm;

    [SerializeField] private Image m_Image;
    private List<Sprite> m_Props = new List<Sprite>();

    private int m_CurrentPropIndex;

    private void Start()
    {
        m_Props = MapEditor.s_Instance.PropSprites;
        m_CurrentPropIndex = 0;

        UpdateProp();
    }   

    public void MoveLeft()
    {
        if (m_CurrentPropIndex <= 0)
            m_CurrentPropIndex = m_Props.Count - 1;
        else
            m_CurrentPropIndex--;

        UpdateProp();
    }

    public void MoveRight()
    {
        if (m_CurrentPropIndex >= m_Props.Count - 1)
            m_CurrentPropIndex = 0;
        else
            m_CurrentPropIndex++;

        UpdateProp();
    }

    public void ConfirmSelection()
    {
        string[] fileExtensions = { ".png", ".jpg" };

        string path = AssetDatabase.GetAssetPath(m_Image.sprite);
        path = path.Remove(0, 17); // Remove "/Assets/Resources/" from path string

        for (int i = 0; i < fileExtensions.Length; i++)
        {
            if (path.Contains(fileExtensions[i]))
                path = path.Replace(fileExtensions[i], "");
        }
        //path = path.Replace(".png", "");

        if (s_OnPropSelectorConfirm != null) s_OnPropSelectorConfirm(path);
    }

    private void UpdateProp()
    {
        m_Image.sprite = m_Props[m_CurrentPropIndex];
        m_Image.SetNativeSize();
    }
}
