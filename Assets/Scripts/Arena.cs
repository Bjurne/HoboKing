using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Arena : MonoBehaviour
{
    [SerializeField] int numberOfMasterNodes = default;
    [SerializeField] int numberOfTiles = default;
    [SerializeField] float tileSize = default;
    [SerializeField] float chanceToLeaveNodesInactive = default;

    [SerializeField] float nodeCreationTimer = default;
    [SerializeField] float tileCreationTimer = default;

    [SerializeField] GameObject arenaNodePrefab = default;
    [SerializeField] Transform nodeContainer = default;
    [SerializeField] GameObject arenaTilePrefab = default;
    [SerializeField] Transform tileContainer = default;


    private List<Vector2> masterNodePositions;
    private List<ArenaNode> nodes;
    internal List<ArenaTile> tiles;

    private void Start()
    {
        masterNodePositions = new List<Vector2>();
        nodes = new List<ArenaNode>();
        tiles = new List<ArenaTile>();
        StartCoroutine(GenerateArena());
    }

    private IEnumerator GenerateArena()
    {
        var apothem = tileSize / 2f;
        var centerToVertex = tileSize * 0.5774f;

        GenerateMasterNodes(apothem, centerToVertex);
        yield return GenerateNodes(apothem, centerToVertex);
        SetupNodeNeighbours();
        //GenerateTilesStringy();
        yield return GenerateTilesFromCenter();
        SetupTileNeighbours();
        //var indexes = new List<Vector2> { new Vector2(1, 2), new Vector2(0, 1), new Vector2(0, 2), new Vector2(1, 1) };
        //for (int i = 0; i < indexes.Count; i++)
        //{
        //    var tile = GetTileByIndex(indexes[i]);
        //    if (tile != null)
        //        tile.AddOccupyingMarker(GameLocalization.Instance.GetMarkerPrefab());
        //}
        //yield return CountFinalScores();
    }

    private void GenerateMasterNodes(float apothem, float centerToVertex)
    {
        for (int i = 0; i < numberOfMasterNodes; i++)
        {
            var offsetIndex = Mathf.Ceil(i / 2f);
            var offsetX = (centerToVertex * 1.5f) * offsetIndex;
            var offsetY = apothem * offsetIndex;

            if (i % 2 != 0)
            {
                offsetX *= -1;
                offsetY *= -1;
            }

            var masterNodePosition = new Vector2(offsetX, offsetY);
            masterNodePositions.Add(masterNodePosition);
        }
    }

    private IEnumerator GenerateNodes(float apothem, float centerToVertex)
    {
        for (int i = 0; i < masterNodePositions.Count; i++)
        {
            var idX = Mathf.Ceil(i / 2f);

            if (i % 2 != 0)
                idX = -idX;

            //for (int j = 0; j < numberOfNodes / masterNodePositions.Count; j++)
            var nodesInRow = numberOfMasterNodes - Mathf.Ceil(i / 2f);
            for (int j = 0; j < nodesInRow; j++)
            {
                var offsetIndex = Mathf.Ceil(j / 2f);
                var offsetX = -(centerToVertex * 1.5f) * offsetIndex;
                var offsetY = apothem * offsetIndex;
                var idY = offsetIndex;

                if (j % 2 != 0)
                {
                    offsetX *= -1;
                    offsetY *= -1;
                    idY = -offsetIndex;
                }

                var nodePosition = new Vector2(masterNodePositions[i].x + offsetX, masterNodePositions[i].y + offsetY);

                var newNode = Instantiate(arenaNodePrefab, nodePosition, Quaternion.identity, nodeContainer);
                var node = newNode?.GetComponent<ArenaNode>();
                var id = new Vector2(idX, idY);
                node.SetIndex(id);
                nodes.Add(node);

                yield return new WaitForSeconds(nodeCreationTimer);
            }
        }
        yield return null;
    }

    private void GenerateTilesStringy()
    {
        for (int i = 0; i < Mathf.Min(numberOfTiles, nodes.Count - 1); i++)
        {
            var index = i;
            if (index > numberOfMasterNodes / 2f)
                index += Mathf.FloorToInt(numberOfMasterNodes / 2f);
            //index += UnityEngine.Random.Range(0,3);
            if (index >= nodes.Count)
                return;
            var node = nodes[index].transform.position;
            var arenaTile = Instantiate(arenaTilePrefab, node, Quaternion.identity, tileContainer);
            nodes[index].SetActive(true, arenaTile.GetComponent<ArenaTile>());
        }
    }

    private IEnumerator GenerateTilesFromCenter()
    {
        if (numberOfTiles >= nodes.Count)
            numberOfTiles = nodes.Count - 1;

        var tilesPlaced = 0;
        var indexFromCenter = 2;

        //List<ArenaNode> sortedNodeList = nodes.OrderBy(o => Mathf.Abs(o.Index.x) != Mathf.Abs(o.Index.y)).ThenBy(o => Mathf.Abs(o.Index.x) + Mathf.Abs(o.Index.y)).ToList();
        List<ArenaNode> sortedNodeList = nodes.OrderBy(o => o.transform.position.magnitude).ThenBy(o => Mathf.Abs(o.Index.Index.x) != Mathf.Abs(o.Index.Index.y)).ToList();


        var failSafe = numberOfTiles + 50;
        for (int i = 0; i < numberOfTiles; failSafe--)
        {
            if (failSafe < 0)
                break;

            //Debug.Log($"Node: {i}, attempts remaining: {failSafe}");
            foreach (ArenaNode node in sortedNodeList)
            {
                if (node.TargetableStatus.isActive || node.LeftInactive)
                {
                    continue;
                }

                var randomPercentage = UnityEngine.Random.Range(0f, 100f);
                if (randomPercentage < chanceToLeaveNodesInactive)
                {
                    node.SetLeftInactive(true);
                    continue;
                }

                var idSum = Mathf.Abs(node.Index.Index.x) + Mathf.Abs(node.Index.Index.y);
                if (idSum <= indexFromCenter)
                {
                    //var arenaTile = Instantiate(arenaTilePrefab, node.transform.position, Quaternion.identity, tileContainer);
                    //var tile = arenaTile.GetComponent<ArenaTile>();
                    //node.SetActive(true, tile);
                    //tiles.Add(tile);
                    AddTile(node);
                    tilesPlaced++;
                    i++;
                    if (tilesPlaced % 8 == 0)
                        indexFromCenter++;
                    break;
                }
            }
            yield return new WaitForSeconds(tileCreationTimer);
        }
    }

    private void SetupNodeNeighbours()
    {
        foreach (ArenaNode node in nodes)
        {
            var neighbours = new List<ArenaNode>();

            foreach (ArenaNode otherNode in nodes)
            {
                if (node == otherNode)
                    continue;
                if (Vector2.Distance(node.transform.position, otherNode.transform.position) <= tileSize * 1.5f)
                    neighbours.Add(otherNode);
            }

            node.SetNeighbours(neighbours.ToArray());
            neighbours.Clear();
        }
    }

    private void SetupTileNeighbours()
    {
        foreach (ArenaTile tile in tiles)
        {
            var neighbours = new List<ArenaTile>();

            foreach (ArenaTile otherTile in tiles)
            {
                if (tile == otherTile)
                    continue;
                if (Vector2.Distance(tile.transform.position, otherTile.transform.position) <= tileSize * 1.5f)
                    neighbours.Add(otherTile);
            }

            tile.SetNeighbours(neighbours.ToArray());
            neighbours.Clear();
        }
    }

    internal void AddTile(ArenaNode arenaNode)
    {
        if (!arenaNode.TargetableStatus.isOccupied)
        {
            var arenaTile = Instantiate(arenaTilePrefab, arenaNode.transform.position, Quaternion.identity, tileContainer);
            var tile = arenaTile.GetComponent<ArenaTile>();
            arenaNode.SetActive(true, tile);
            tiles.Add(tile);

            SetupTileNeighbours(); // TODO: Can we set neighbours for newly created tiles (and their neighbours) without running Setup again?
        }
    }

    private ArenaTile GetTileByIndex(Vector2 index)
    {
        ArenaTile arenaTile = null;
        foreach (ArenaTile tile in tiles)
        {
            if (tile.Index.Index == index)
            {
                arenaTile = tile;
                break;
            }
        }
        return arenaTile;
    }

    internal void DebugScoreCount()
    {
        StartCoroutine(CountFinalScores());
    }

    private IEnumerator CountFinalScores()
    {
        yield return new WaitForEndOfFrame();
        var allPlayers = GameStateManager.Instance.GetAllPlayers();
        var scores = new int[allPlayers.Length];
        var allMarkers = FindObjectsOfType<Marker>();

        for (int i = 0; i < allPlayers.Length; i++)
        {
            foreach (Marker marker in allMarkers)
            {
                if (marker.Player == allPlayers[i] && marker.ParentTile.IsHalfMarkedByPlayer((PlayerIndex)i) == false)
                {
                    scores[(int)allPlayers[i].PlayerIndex]++;
                    //marker.gameObject.transform.localScale *= 0.9f;
                    //var originalColor = marker.GetComponent<Image>().color;
                    //marker.ParentTile.Mark();
                    //yield return new WaitForSeconds(1f);
                    var tiles = new List<ArenaTile>();
                    foreach (ITargetable targetable in marker.ParentTile.Neighbours)
                    {
                        tiles.Add(targetable as ArenaTile);
                    }
                    foreach (ArenaTile tile in tiles)
                    {
                        if (tile.OccupyingMarker != null && tile.OccupyingMarker.GetComponent<Marker>().Player == allPlayers[i] && tile.IsHalfMarkedByPlayer((PlayerIndex)i) == false)
                        {
                            scores[(int)allPlayers[i].PlayerIndex]++;
                            //tile.OccupyingMarker.transform.localScale *= 0.9f;
                            //yield return new WaitForSeconds(1f);
                        }
                    }
                    //yield return new WaitForSeconds(1f);
                }
            }
        }

        for (int i = 0; i < scores.Length; i++)
        {
            Debug.Log($"Player {i} scored {scores[i]}");
        }

        yield return new WaitForSeconds(4f);

        foreach (Marker marker in allMarkers)
        {
            marker.gameObject.transform.localScale = Vector3.one;
        }

        yield return null;
    }
}
