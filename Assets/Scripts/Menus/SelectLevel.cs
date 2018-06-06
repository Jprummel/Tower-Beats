﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectLevel : MonoBehaviour
{

    [SerializeField] private Song[] m_Playlist = new Song[3];

    public void SelectTheLevel()
    {
        Level selectedLevel = ScrollLevels.s_Instance.GetSelectedLevel();

        for (int i = 0; i < selectedLevel.Songs.Length; i++)
        {
            m_Playlist[i].Songname = selectedLevel.Songs[i];
        }
        SongManager.s_Instance.Songs = m_Playlist;
        Sceneloader.s_Instance.LoadScene("Game");
    }
}
