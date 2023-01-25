using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchDraw : MonoBehaviour
{
    Coroutine drawing;
    public GameObject linePrefab;
    public Transform startingPosition;
    public static List<LineRenderer> drawnLineRenderers = new List<LineRenderer>();
    public float scale = 1f;
    bool recording = false;

    [System.Serializable]
    public class Line
    {
        public Vector3[] line;
        public Line(Vector3[] newLine)
        {
            line = newLine;
        }
    }

    [System.Serializable]
    public class AnimationData
    {
        public List<Line> lines;
        public AnimationData(List<LineRenderer> lineRenderers)
        {
            lines = new List<Line>();
            foreach (LineRenderer lineRenderer in lineRenderers)
            {
                Vector3[] points = new Vector3[lineRenderer.positionCount];
                lineRenderer.GetPositions(points);
                lines.Add(new Line(points));
            }
        }
    }

    AnimationData animationData;
    public bool loadMode = true;

    void Update()
    {
        if (loadMode)
        {
            if (!recording)
            {
                LoadJSONFont();
                WriteTextWrapper(startingPosition.position);
                recording = true;
            }
        }
        else
        {
            if (Input.GetKeyDown("a"))
            {
                Debug.Log("Starting drawing");
                recording = true;

            }
            if (Input.GetKeyDown("b"))
            {
                recording = false;
                Debug.Log("Ending drawing");
                FinishLine();
            }

            if (recording && Input.GetMouseButtonDown(0))
            {
                if (EventSystem.current.currentSelectedGameObject == null)
                {
                    StartLine();
                }
            }

            if (recording && Input.GetMouseButtonUp(0))
            {
                if (drawing != null)
                {
                    StopCoroutine(drawing);
                }
            }
        }

    }

    public void StartLine()
    {
        if (drawing != null)
        {
            StopCoroutine(drawing);
        }
        drawing = StartCoroutine(DrawLine());
    }
    public void FinishLine()
    {
        if (drawing != null)
        {
            SaveAnimation();
        }
            
    }


    private void SaveAnimation()
    {

        //ClearLetter();
        animationData = new AnimationData(drawnLineRenderers);
        SaveJSONFile();
        WriteTextWrapper(startingPosition.position);
    }

    private void SaveJSONFile()
    {
        string jsonString = JsonUtility.ToJson(new AnimationData(drawnLineRenderers));
        string fileName = Application.persistentDataPath + "/HandWrittenFont.txt";
        File.WriteAllText(fileName, jsonString);
        PlayerPrefs.SetInt("Saved", 1);
    }


    private void LoadJSONFont()
    {
        string completeFileName = Application.persistentDataPath + "/HandWrittenFont.txt";
        Debug.Log("completeFileName");
        if (File.Exists(completeFileName))
        {
            string fileData = File.ReadAllText(completeFileName);
            animationData = JsonUtility.FromJson<AnimationData>(fileData);
        } else
            Debug.Log("Need a valid name");
    }

    IEnumerator DrawLine()
    {
        GameObject newGameObject = Instantiate(linePrefab, new Vector3(0, 0, 0), Quaternion.identity);
        LineRenderer line = newGameObject.GetComponent<LineRenderer>();
        drawnLineRenderers.Add(line);
        line.positionCount = 0;
        while (true)
        {
            Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            position.z = 0;
            line.positionCount++;
            line.SetPosition(line.positionCount - 1, position);
            yield return null;
        }
    }


    #region WriteText
    public void WriteTextWrapper(Vector3 startingPosition)
    {
        StartCoroutine(WriteText(startingPosition)); //animationData.lines
    }


    private bool drawingLetter = false;
    IEnumerator WriteText(Vector3 startingPosition)
    {

        foreach(Line line in animationData.lines)
        {
            GameObject newLineGameObject = Instantiate(linePrefab, startingPosition, Quaternion.identity);
            newLineGameObject.transform.rotation = Quaternion.Euler(0, 180, 90);
            LineRenderer lineRenderer = newLineGameObject.GetComponent<LineRenderer>();
            lineRenderer.positionCount = 0;
            lineRenderer.widthMultiplier = 0.1f;
            int pointIndex = 0;

            while (pointIndex < line.line.Length)
            {
                lineRenderer.positionCount++;
                lineRenderer.SetPosition(pointIndex, line.line[pointIndex] * scale /*Vector3.Scale(line.line[pointIndex], new Vector3(scale, scale, 0))*/);
                pointIndex++;
                yield return null;
            }
            yield return null;
        }
        // yield return new WaitForSeconds(.1f);
        drawingLetter = false;
        yield return null;

    }
    #endregion
}
