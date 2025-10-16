using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] protected MeshFilter mesh;
    [SerializeField] protected MeshRenderer meshRenderer;
    [SerializeField] protected TileData tileData;

    private int yRotation = 0;
    private int oneQuarterRotation = 0;
    private int oneHalfRotation = 0;
    private int diagonalRoation = 0;
    private int threeQuarterRotation = 90;

    public void CalculateTileVisual(List<int> objectIds)
    {
        //Debug.Log(gameObject.name);

        //foreach (var id in objectIds)
        //{
        //    Debug.Log("Object ID in tile: " + id);
        //}

        int mainObjectCount = objectIds.FindAll(id => id == tileData.mainObject.ID).Count;

        GameObject chosenVisualTile = null;
        switch (mainObjectCount)
        {
            case 0:
            {
                Vector3Int gridPosition = PlacementSystem.Instance.DualGrid.WorldToCell(transform.position);
                PlacementSystem.Instance.RemoveObjectInDualGrid(gridPosition, tileData.mainObject.gridType);
                break;
            }
            case 1:
            {
                chosenVisualTile = tileData.tile_quarter_1;
                yRotation = oneQuarterRotation - 90 * objectIds.IndexOf(tileData.mainObject.ID);
                break;
            }
            case 2:
            {
                int firstIndex = objectIds.IndexOf(tileData.mainObject.ID);
                int lastIndex = objectIds.LastIndexOf(tileData.mainObject.ID);

                if (Mathf.Abs(firstIndex - lastIndex) == 2)
                {
                    chosenVisualTile = tileData.tile_quarter_1_3;
                    yRotation = diagonalRoation + (-90 * firstIndex);
                }
                else
                {
                    chosenVisualTile = tileData.tile_half;
                    
                    if (firstIndex == 0 && lastIndex == 3)
                    {
                        yRotation = oneHalfRotation + (-90 * (lastIndex + 1));
                    }
                    else
                    {
                        yRotation = oneHalfRotation + (-90 * (firstIndex + 1));
                    }
                }

                break;
            }
            case 3:
            {
                chosenVisualTile = tileData.tile_quarter_1_2_3;
                int otherIndex = objectIds.FindIndex(id => id != tileData.mainObject.ID);
                yRotation = threeQuarterRotation + (-90 * otherIndex);
                break;
            }
            case 4:
            {
                chosenVisualTile = tileData.tile_full;
                break;
            }
            default:
                break;
        }

        if (chosenVisualTile != null)
        {
            UpdateTileVisual(chosenVisualTile);
        }
        else
        {

        }
    }

    private void UpdateTileVisual(GameObject visualTile)
    {
        mesh.mesh = visualTile.GetComponent<MeshFilter>().sharedMesh;
        meshRenderer.materials = visualTile.GetComponent<MeshRenderer>().sharedMaterials;
        Debug.Log(yRotation);
        mesh.gameObject.transform.localEulerAngles = new Vector3(0, yRotation, 0);
    }
}
