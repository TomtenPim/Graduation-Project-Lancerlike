using System;
using System.Collections;
using System.Drawing;
using UnityEngine;

public class BattlefieldCamera : MonoBehaviour
{
    public Camera thisCamera;
    GameObject currentFocus;
    [SerializeField] float cameraSpeed;

    [SerializeField] float xBoundsMin;
    [SerializeField] float xBoundsMax;
    [SerializeField] float zBoundsMin;
    [SerializeField] float zBoundsMax;

    bool lerping = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        MoveCamera();
    }

    private void MoveCamera()
    {
        if (lerping)
        {
            return;   
        }


        Vector3 movmentVector = Vector3.zero;

        if (Input.GetKey(KeyCode.W) && transform.position.z < zBoundsMax)
        {
            movmentVector += new Vector3(0, 0, 1);
        }
        if (Input.GetKey(KeyCode.A) && transform.position.x > xBoundsMin)
        {
            movmentVector += new Vector3(-1, 0, 0);
        }
        if (Input.GetKey(KeyCode.S) && transform.position.z > zBoundsMin)
        {
            movmentVector += new Vector3(0, 0, -1);
        }
        if (Input.GetKey(KeyCode.D) && transform.position.x < xBoundsMax)
        {
            movmentVector += new Vector3(1, 0, 0);
        }


        this.transform.position += movmentVector.normalized * Time.deltaTime * cameraSpeed;

    }

    public void ChangeFocus(GameObject newFocus)
    {
        Debug.Log("BattledieldCamera - ChangeFocus was called");
        if (newFocus == null) { return; }

        //currentFocus = newFocus;
        StartCoroutine(MoveSmoothlyToNewFocus(transform.position, newFocus.transform.position, 0.1f));

    }


    IEnumerator MoveSmoothlyToNewFocus(Vector3 startPosition, Vector3 endPosition, float timeToFinish)
    {
        float t = 0;

        Vector3 clampedEndPosition = new Vector3(Mathf.Clamp(endPosition.x, xBoundsMin, xBoundsMax), endPosition.y, Mathf.Clamp(endPosition.z, zBoundsMin + 6.16f, zBoundsMax));
        lerping = true;

        while (t/timeToFinish < 1)
        {
            transform.position = new Vector3(
                Mathf.Lerp(startPosition.x, clampedEndPosition.x, t / timeToFinish),
                Mathf.Lerp(startPosition.y, 10f, t / timeToFinish),
                Mathf.Lerp(startPosition.z, clampedEndPosition.z - 6.16f, t / timeToFinish)
            );

            t += Time.fixedDeltaTime;

            yield return new WaitForFixedUpdate();
        }

        lerping = false;
    }
}
