﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSongButtons : MonoBehaviour {

	public void SkipSong()
    {
        SongManager.s_Instance.SkipSong();
    }
}
