﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

#region Enums

/// <summary>
/// Open = buildable, Not_Usable = unusable tile, Path = Enemy AI Path, Occupied = Tile that contains tower
/// </summary>
public enum TileState
{
    OPEN,
    NOT_USABLE,
    PATH,
    OCCUPIED
}

public enum TileVisualState
{
    BASE,
    PATH,
    SELECTED,
    UNSELECTED
}

#endregion

#region Serializables

[System.Serializable]
public struct TileArt
{
    public TileVisualState VisualState;
    public SpriteRenderer VisualStateRenderer;
}

#endregion

public class Tile : MonoBehaviour
{
    #region Variables

    public TileState CurrentState { get; set; }

    public Vector2Int PositionInGrid { get; set; }
    public int X { get { return PositionInGrid.x; } set { PositionInGrid = new Vector2Int(value, PositionInGrid.y); } }
    public int Y { get { return PositionInGrid.y; } set { PositionInGrid = new Vector2Int(PositionInGrid.x, value); } }

    [SerializeField]private List<TileArt> m_TileArt = new List<TileArt>();

    public delegate void TileClicked(Tile tile);
    public static TileClicked s_OnTileClicked;

    public Tower Tower { get; set; } //The tower on this tile

    #endregion

    #region Monobehavior functions
    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        CurrentState = TileState.OPEN;
        Tower = null;
    }

    void OnMouseDown()
    {
        if (!EventSystem.current.IsPointerOverGameObject()) 
        {
            if (s_OnTileClicked != null) s_OnTileClicked(this);
            switch (CurrentState)
        	{
            	case TileState.OPEN:
                    if (PopUpManager.s_Instance != null)
                    {
                        PopUpManager.s_Instance.ShowPopUp(PopUpNames.TOWER_SHOP_MENU, new Vector2(transform.position.x,transform.position.y+1.5f));
                    }
                	//Open tower shop menu
                	break;
            	case TileState.OCCUPIED:
                    //Open tower menu and shows the stats of the tower on this tile
                    if (PopUpManager.s_Instance != null)
                    {
                        PopUpManager.s_Instance.ShowPopUp(PopUpNames.TOWER_MENU, transform.position);
                    }
                	break;
        	}
		}
    }

    #endregion

    #region Tile functions

    /// <summary>
    /// Sets the tile's visual state to the given state
    /// </summary>
    /// <param name="state">State of the tile</param>
    public void SetTileVisualsState(TileVisualState state)
    {
        for (int i = 0; i < m_TileArt.Count; i++)
        {
            if (m_TileArt[i].VisualState == state)
                m_TileArt[i].VisualStateRenderer.enabled = true;
            else
                m_TileArt[i].VisualStateRenderer.enabled = false;
        }
    }

    #endregion
}