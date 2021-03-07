using Newtonsoft.Json;
using RestClient.Core;
using RestClient.Core.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityTemplateProjects;

public class WarehouseCtrl : MonoBehaviour
{
    List<StorageLocation> storageLocations = new List<StorageLocation>();
    List<HandlingUnit> handlingUnits = new List<HandlingUnit>();
    List<TempUnit> tempUnits = new List<TempUnit>();
    public GameObject columnPrefab;
    public GameObject columnPrefab_1;

    public GameObject aisle;
    public Sprite buttonSprite;
    public GameObject canvas;

    public GameObject testCube;
    public GameObject camera;

    private List<int> handlingUnitIdsToBeCollected;

    private List<int> handlingUnitButtonIdList = new List<int>();

    private List<HandlingUnitTracker> handlingUnitTrackerList = new List<HandlingUnitTracker>();

    [SerializeField]
    private string baseUrl = "https://localhost:44335/api/storagelocations";


    private GameObject popUp;

    private int nextItemIndex = 0;

    GameObject startPickingButton;
    GameObject informationText;
    GameObject inputField;
    GameObject readButton;

    void Start()
    {
        handlingUnitIdsToBeCollected = new List<int>
        {
            5798270, // 5798270	32520096	0	5673606	181
            5556031, // 5556031	99278130	0	5821096	186
            4353198  // 4353198	22353917	0	4353452	230
        };

        SetUIComponent();

        SetStorageLocations();
        //PostHandlingUnits();
        //StartCoroutine(RestWebClient.Instance.HttpGet($"{baseUrl}api/storagelocations", (r) => OnRequestComplete(r)));
        StartCoroutine(RestWebClient.Instance.HttpGet($"{baseUrl}api/handlingunits", (r) => OnHandlingUnitRequestComplete(r)));

        /*
        SetHandlingUnits();
        ActivateStorageLocations();
        ActivateHandlingUnitsAndBarcodes();
        CreateStorageLocationsButtons();
        CreateHandlingUnitsButtons();
        ActivateStorageLocationBarcodes();
        */
        

        // setup the request header

        //SetHandlingUnits();
        //Debug.Log(handlingUnits.Count.ToString());
    }

    bool tested = false;

    
    void Update()
    {
        if (!tested)
        {
            StartCoroutine(RestWebClient.Instance.HttpGet($"{baseUrl}api/tempunits", (r) => OnTempUnitRequestComplete(r)));
        }
    }
    
    private void SetUIComponent()
    {
        popUp = canvas.transform.GetChild(0).GetChild(3).gameObject;

        startPickingButton = canvas.transform.GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(1).gameObject;
        informationText = canvas.transform.GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(2).gameObject;
        inputField = canvas.transform.GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(3).gameObject;
        readButton = canvas.transform.GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(4).gameObject;
    }

    public void CreateStorageLocationButton(Transform parent, string buttonText, int id)
    {
        GameObject button = new GameObject();
        GameObject text = new GameObject();

        button.transform.parent = parent;
        button.AddComponent<RectTransform>();
        button.AddComponent<Button>();
        button.name = buttonText;

        text.transform.parent = button.transform;
        text.AddComponent<RectTransform>();
        text.AddComponent<Text>();

        text.GetComponent<Text>().text = buttonText;
        text.GetComponent<Text>().fontStyle = FontStyle.Bold;
        text.GetComponent<Text>().fontSize = 22;
        text.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
        text.GetComponent<Text>().color = new Color32(0x32, 0x32, 0x32, 0xff);
        text.GetComponent<Text>().verticalOverflow = VerticalWrapMode.Overflow;
        text.GetComponent<Text>().font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        text.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
        text.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);
        text.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
        text.GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);
        text.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);

        button.AddComponent<Image>();
        button.GetComponent<Image>().sprite = buttonSprite;
        button.GetComponent<Image>().color = new Color32(0x6f, 0x6f, 0x6f, 0xff);

        button.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 360);
        button.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 25);

        button.GetComponent<Button>().onClick.AddListener(delegate () { OnStorageLocationButtonClicked(id); });
    }
    
    public void CreateHandlingUnitButton(Transform parent, string buttonText, int id)
    {
        GameObject button = new GameObject();
        GameObject text = new GameObject();

        button.transform.parent = parent;
        button.AddComponent<RectTransform>();
        button.AddComponent<Button>();
        button.name = buttonText;

        text.transform.parent = button.transform;
        text.AddComponent<RectTransform>();
        text.AddComponent<Text>();

        text.GetComponent<Text>().text = buttonText;
        text.GetComponent<Text>().fontStyle = FontStyle.Bold;
        text.GetComponent<Text>().fontSize = 22;
        text.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
        text.GetComponent<Text>().color = new Color32(0x32, 0x32, 0x32, 0xff);
        text.GetComponent<Text>().verticalOverflow = VerticalWrapMode.Overflow;
        text.GetComponent<Text>().font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        text.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
        text.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);
        text.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
        text.GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);
        text.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);

        button.AddComponent<Image>();
        button.GetComponent<Image>().sprite = buttonSprite;
        button.GetComponent<Image>().color = new Color32(0x6f, 0x6f, 0x6f, 0xff);

        button.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 360);
        button.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 25);

        button.GetComponent<Button>().onClick.AddListener(delegate () { OnHandlingUnitButtonClicked(button); });
        //button.GetComponent<Button>().onClick.AddListener(delegate () { testt(button); });

        button.AddComponent<HandlingUnitButton>();
        button.GetComponent<HandlingUnitButton>().HandlingUnitId = id;

        handlingUnitButtonIdList.Add(id);
    }

    public void OnStartPickingClicked()
    {
        startPickingButton.SetActive(false);
        informationText.SetActive(true);
        inputField.SetActive(true);
        readButton.SetActive(true);

        nextItemIndex = 0;

        HandlingUnit unit = handlingUnits.Where(p => p.HandlingUnitId == handlingUnitIdsToBeCollected[nextItemIndex]).FirstOrDefault();
        StorageLocation location = storageLocations.Where(p => p.StorageLocationId == unit.StorageLocationId).FirstOrDefault();
        string storageLocationBarcode = location.StorageLocationBarcode;

        informationText.GetComponent<Text>().text = "Go to location " + storageLocationBarcode;
    }

    private void OnStorageLocationButtonClicked(int id)
    {

        int index = storageLocations.IndexOf(storageLocations.Single(i => i.StorageLocationId == id));
        Transform buttonListContentTransform = canvas.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).gameObject.transform;

        foreach (Transform childButton in buttonListContentTransform)
            childButton.GetComponent<Image>().color = new Color32(0x6f, 0x6f, 0x6f, 0xff);

        buttonListContentTransform.GetChild(index).GetComponent<Image>().color = new Color32(0xa2, 0xa2, 0xa2, 0xff);

        float y = 0;
        float z = 0;
        float x = 0;

        Quaternion rotation = Quaternion.identity;
        

        if (id <= storageLocations.Count / 2)
        {
            y = ((id - 1) % 5) * 1.95f + 1.35f;
            z = -((id - 1) / 5 + 0.5f) * 2.8f / 3;
            x = -2.025f - 0.8f;

            rotation.eulerAngles = new Vector3(12f, 90f, 0f);
        }
        else
        {
            y = ((id - storageLocations.Count / 2 - 1) % 5) * 1.95f + 1.35f;
            z = -((id - storageLocations.Count / 2 - 1) / 5 + 0.5f) * 2.8f / 3;
            x = -2.025f + 0.8f;

            rotation.eulerAngles = new Vector3(12f, -90f, 0f);
        }

        camera.GetComponent<SimpleCameraController>().SetStartAndEndPositionAndRotation(new Vector3(x, y, z), rotation);
        camera.GetComponent<SimpleCameraController>().SetButtonsNotPressed();
    }

    int operationState = 0;

    public void OnOperationButtonClicked()
    {
        string inputText = inputField.transform.GetChild(2).GetComponent<Text>().text;
        inputText = inputText.ToUpper();

        HandlingUnit unit = handlingUnits.Where(p => p.HandlingUnitId == handlingUnitIdsToBeCollected[nextItemIndex]).FirstOrDefault();
        StorageLocation location = storageLocations.Where(p => p.StorageLocationId == unit.StorageLocationId).FirstOrDefault();
        string storageLocationBarcode = location.StorageLocationBarcode;
        string handlingUnitCode = unit.HandlingUnitCode;

        if (operationState == 0)
        {
            if (!string.IsNullOrEmpty(inputText))
            {
                StorageLocation match = storageLocations.Where(p => p.StorageLocationBarcode == inputText).FirstOrDefault();

                if (match != null)
                {

                    if (match.StorageLocationBarcode.Equals(storageLocationBarcode))
                    {
                        OnStorageLocationButtonClicked(Convert.ToInt32(match.StorageLocationId));
                        informationText.GetComponent<Text>().text = "You are at " + storageLocationBarcode + ". Now pick the unit " + unit.HandlingUnitCode + ".";
                        inputField.transform.GetChild(1).GetComponent<Text>().text = "Enter handling unit code...";
                        inputField.GetComponent<InputField>().Select();
                        inputField.GetComponent<InputField>().text = "";
                        readButton.transform.GetChild(0).GetComponent<Text>().text = "Read Handling Unit";

                        operationState = 1;
                    }
                    else
                    {
                        popUp.transform.GetChild(0).GetComponent<Text>().text = "Locations do not match !";
                        popUp.SetActive(true);
                    }
                }
                else
                {
                    popUp.transform.GetChild(0).GetComponent<Text>().text = "There is no such location !";
                    popUp.SetActive(true);
                }
            }
        }
        else if (operationState == 1)
        {
            if (!string.IsNullOrEmpty(inputText))
            {
                HandlingUnit match = handlingUnits.Where(p => p.HandlingUnitCode == inputText).FirstOrDefault();

                if (match != null)
                {
                    if (match.HandlingUnitCode.Equals(handlingUnitCode))
                    {
                        if(nextItemIndex < handlingUnitIdsToBeCollected.Count - 1)
                        {
                            /* For the next handling unit */
                            HandlingUnit nextUnit = handlingUnits.Where(p => p.HandlingUnitId == handlingUnitIdsToBeCollected[nextItemIndex + 1]).FirstOrDefault();
                            StorageLocation nextLocation = storageLocations.Where(p => p.StorageLocationId == nextUnit.StorageLocationId).FirstOrDefault();
                            string nextStorageLocationBarcode = nextLocation.StorageLocationBarcode;

                            informationText.GetComponent<Text>().text = "You picked " + handlingUnitCode + ". Now go to location " + nextStorageLocationBarcode + ".";
                            inputField.transform.GetChild(1).GetComponent<Text>().text = "Enter storage location...";
                            inputField.GetComponent<InputField>().Select();
                            inputField.GetComponent<InputField>().text = "";
                            readButton.transform.GetChild(0).GetComponent<Text>().text = "Read Location";

                            operationState = 0;

                            nextItemIndex++;

                            
                        }
                        else
                        {
                            informationText.GetComponent<Text>().text = "You picked " + handlingUnitCode + ".";
                            inputField.GetComponent<InputField>().Select();
                            inputField.GetComponent<InputField>().text = "";

                            /////// butonlar aktive pop up
                            ///
                            popUp.SetActive(true);
                            popUp.transform.GetChild(0).GetComponent<Text>().text = "You picked all handling units successfully";
                            startPickingButton.SetActive(true);
                            informationText.SetActive(false);
                            inputField.SetActive(false);
                            readButton.SetActive(false);
                        }

                        CollectBox(match);
                    }
                    else
                    {
                        popUp.transform.GetChild(0).GetComponent<Text>().text = "Handling units do not match !";
                        popUp.SetActive(true);
                    }
                }
                else
                {
                    popUp.transform.GetChild(0).GetComponent<Text>().text = "There is no such handling unit !";
                    popUp.SetActive(true);
                }
            }
        }
    }
    /*
    public void CollectBox(HandlingUnit handlingUnit)
    {
        int storageLocationId = Convert.ToInt32(handlingUnit.StorageLocationId);

        int storageLocationUnitId = (storageLocationId - 1) / 15 + 1;//location.StorageLocationUnitId;

        GameObject locationUnit = aisle.transform.GetChild(storageLocationUnitId - 1).gameObject;

        GameObject palette = locationUnit.transform.GetChild(Convert.ToInt32((storageLocationId - 1) % 15)).gameObject;
        GameObject box = palette.transform.GetChild(lastHandlingUnitBoxIndex).gameObject;

        GameObject boxToBecomeInactive;

        int lastBoxIndex = handlingUnitTrackerList[lastHandlingUnitTrackIndex].handlingUnitBoxes.Count - 1;

        if (handlingUnitTrackerList[lastHandlingUnitTrackIndex].handlingUnitBoxes.Count - 1 > lastHandlingUnitBoxIndex)
            box.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>().text = handlingUnitTrackerList[lastHandlingUnitTrackIndex].handlingUnitBoxes[lastBoxIndex].HandlingUnitCode;

        boxToBecomeInactive = palette.transform.GetChild(lastBoxIndex).gameObject;
        boxToBecomeInactive.SetActive(false);

        DeleteButton(handlingUnitTrackerList[lastHandlingUnitTrackIndex].handlingUnitBoxes[lastHandlingUnitBoxIndex].HandlingUnitId);

        handlingUnitTrackerList[lastHandlingUnitTrackIndex].handlingUnitBoxes[lastHandlingUnitBoxIndex] = handlingUnitTrackerList[lastHandlingUnitTrackIndex].handlingUnitBoxes[lastBoxIndex];
        handlingUnitTrackerList[lastHandlingUnitTrackIndex].handlingUnitBoxes.RemoveAt(lastBoxIndex);

    }
    */
    private void CollectBox(HandlingUnit handlingUnit)
    {
        int handlingUnitId = Convert.ToInt32(handlingUnit.HandlingUnitId);

        for (int i = 0; i < handlingUnitTrackerList.Count; i++)
        {
            if (handlingUnitTrackerList[i].handlingUnitPalette != null)
            {
                if (handlingUnitId == handlingUnitTrackerList[i].handlingUnitPalette.HandlingUnitId)
                {
                    goto End;
                }
            }

            for (int j = 0; j < handlingUnitTrackerList[i].handlingUnitBoxes.Count; j++)
            {
                if (handlingUnitId == handlingUnitTrackerList[i].handlingUnitBoxes[j].HandlingUnitId)
                {
                    lastHandlingUnitTrackIndex = i;
                    lastHandlingUnitBoxIndex = j;
                    goto End;
                }
            }
        }
        End:

        int storageLocationId = Convert.ToInt32(handlingUnit.StorageLocationId);

        int storageLocationUnitId = (storageLocationId - 1) / 15 + 1;//location.StorageLocationUnitId;

        GameObject locationUnit = aisle.transform.GetChild(storageLocationUnitId - 1).gameObject;

        GameObject palette = locationUnit.transform.GetChild(Convert.ToInt32((storageLocationId - 1) % 15)).gameObject;
        GameObject box = palette.transform.GetChild(lastHandlingUnitBoxIndex).gameObject;

        GameObject boxToBecomeInactive;

        int lastBoxIndex = handlingUnitTrackerList[lastHandlingUnitTrackIndex].handlingUnitBoxes.Count - 1;

        if (handlingUnitTrackerList[lastHandlingUnitTrackIndex].handlingUnitBoxes.Count - 1 > lastHandlingUnitBoxIndex)
            box.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>().text = handlingUnitTrackerList[lastHandlingUnitTrackIndex].handlingUnitBoxes[lastBoxIndex].HandlingUnitCode;

        boxToBecomeInactive = palette.transform.GetChild(lastBoxIndex).gameObject;
        boxToBecomeInactive.SetActive(false);

        DeleteButton(handlingUnitTrackerList[lastHandlingUnitTrackIndex].handlingUnitBoxes[lastHandlingUnitBoxIndex].HandlingUnitId);

        handlingUnitTrackerList[lastHandlingUnitTrackIndex].handlingUnitBoxes[lastHandlingUnitBoxIndex] = handlingUnitTrackerList[lastHandlingUnitTrackIndex].handlingUnitBoxes[lastBoxIndex];
        handlingUnitTrackerList[lastHandlingUnitTrackIndex].handlingUnitBoxes.RemoveAt(lastBoxIndex);
    }

    public void OnPopUpOkButtonClicked()
    {
        popUp.SetActive(false);
    }

    int lastHandlingUnitTrackIndex;
    int lastHandlingUnitBoxIndex;
    private void OnHandlingUnitButtonClicked(GameObject button)
    {
        int id = button.GetComponent<HandlingUnitButton>().HandlingUnitId;

        bool paletteClicked = false;

        for (int i = 0; i < handlingUnitTrackerList.Count; i++)
        {
            if (handlingUnitTrackerList[i].handlingUnitPalette != null)
            {
                if(id == handlingUnitTrackerList[i].handlingUnitPalette.HandlingUnitId)
                {
                    paletteClicked = true;
                    OnStorageLocationButtonClicked(Convert.ToInt32(handlingUnitTrackerList[i].handlingUnitPalette.StorageLocationId));
                    goto End;
                }
            }

            for (int j = 0; j < handlingUnitTrackerList[i].handlingUnitBoxes.Count; j++)
            {
                if(id == handlingUnitTrackerList[i].handlingUnitBoxes[j].HandlingUnitId)
                {
                    lastHandlingUnitTrackIndex = i;
                    lastHandlingUnitBoxIndex = j;
                    OnStorageLocationButtonClicked(Convert.ToInt32(handlingUnitTrackerList[i].handlingUnitBoxes[j].StorageLocationId));
                    goto End;
                }
            }
        }
        End:

        Transform buttonListContentTransform = canvas.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(0).gameObject.transform;

        foreach (Transform childButton in buttonListContentTransform)
            childButton.GetComponent<Image>().color = new Color32(0x6f, 0x6f, 0x6f, 0xff);

        button.GetComponent<Image>().color = new Color32(0xa2, 0xa2, 0xa2, 0xff);

        GameObject deleteButton = canvas.transform.GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(0).gameObject;

        if(!paletteClicked)
            deleteButton.SetActive(true);
        else
            deleteButton.SetActive(false);
    }

    private void DeleteButton(long handlingUnitId)
    {
        int index = handlingUnitButtonIdList.IndexOf(Convert.ToInt32(handlingUnitId));
        handlingUnitButtonIdList.RemoveAt(index);

        GameObject handlingUnitListContent = canvas.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(index).gameObject;
        Destroy(handlingUnitListContent);
    }

    public void OnDeleteButtonClicked()
    {
        int storageLocationId = Convert.ToInt32(handlingUnitTrackerList[lastHandlingUnitTrackIndex].storageLocationId);

        int storageLocationUnitId = (storageLocationId - 1) / 15 + 1;//location.StorageLocationUnitId;

        GameObject locationUnit = aisle.transform.GetChild(storageLocationUnitId - 1).gameObject;

        GameObject palette = locationUnit.transform.GetChild(Convert.ToInt32((storageLocationId - 1) % 15)).gameObject;
        GameObject box = palette.transform.GetChild(lastHandlingUnitBoxIndex).gameObject;

        GameObject boxToBecomeInactive;

        int lastBoxIndex = handlingUnitTrackerList[lastHandlingUnitTrackIndex].handlingUnitBoxes.Count - 1;

        if (handlingUnitTrackerList[lastHandlingUnitTrackIndex].handlingUnitBoxes.Count - 1 > lastHandlingUnitBoxIndex)
            box.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>().text = handlingUnitTrackerList[lastHandlingUnitTrackIndex].handlingUnitBoxes[lastBoxIndex].HandlingUnitCode;
            
        boxToBecomeInactive = palette.transform.GetChild(lastBoxIndex).gameObject;
        boxToBecomeInactive.SetActive(false);

        DeleteButton(handlingUnitTrackerList[lastHandlingUnitTrackIndex].handlingUnitBoxes[lastHandlingUnitBoxIndex].HandlingUnitId);

        handlingUnitTrackerList[lastHandlingUnitTrackIndex].handlingUnitBoxes[lastHandlingUnitBoxIndex] = handlingUnitTrackerList[lastHandlingUnitTrackIndex].handlingUnitBoxes[lastBoxIndex];
        handlingUnitTrackerList[lastHandlingUnitTrackIndex].handlingUnitBoxes.RemoveAt(lastBoxIndex);

    }

    public void OnEmptyButtonClicked()
    {
        Transform handlingUnitButtonListContentTransform = canvas.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(0).gameObject.transform;

        foreach (Transform childButton in handlingUnitButtonListContentTransform)
            childButton.GetComponent<Image>().color = new Color32(0x6f, 0x6f, 0x6f, 0xff);

        Transform storageLocationButtonListContentTransform = canvas.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).gameObject.transform;

        foreach (Transform childButton in storageLocationButtonListContentTransform)
            childButton.GetComponent<Image>().color = new Color32(0x6f, 0x6f, 0x6f, 0xff);

        GameObject deleteButton = canvas.transform.GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(0).gameObject;
        deleteButton.SetActive(false);

        popUp.SetActive(false);
    }

    private void CreateStorageLocationsButtons()
    {
        Transform buttonListContentTransform = canvas.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).gameObject.transform;

        for(int i = 0; i < storageLocations.Count; i++)
        {
            CreateStorageLocationButton(buttonListContentTransform, storageLocations[i].StorageLocationBarcode, Convert.ToInt32(storageLocations[i].StorageLocationId));
        }
    }

    private void CreateHandlingUnitsButtons()
    {
        Transform handlingUnitListContentTransform = canvas.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(0).gameObject.transform;

        for (int i = 0; i < handlingUnitTrackerList.Count; i++)
        {
            if(handlingUnitTrackerList[i].handlingUnitPalette != null)
            {
                CreateHandlingUnitButton(handlingUnitListContentTransform, handlingUnitTrackerList[i].handlingUnitPalette.HandlingUnitCode + " - " +
                    handlingUnitTrackerList[i].handlingUnitPalette.StorageLocationId,
                    Convert.ToInt32(handlingUnitTrackerList[i].handlingUnitPalette.HandlingUnitId));
            }

            for(int j = 0; j < handlingUnitTrackerList[i].handlingUnitBoxes.Count; j++)
            {
                CreateHandlingUnitButton(handlingUnitListContentTransform, handlingUnitTrackerList[i].handlingUnitBoxes[j].HandlingUnitCode + " - " + 
                    handlingUnitTrackerList[i].handlingUnitBoxes[j].StorageLocationId,
                    Convert.ToInt32(handlingUnitTrackerList[i].handlingUnitBoxes[j].HandlingUnitId));
            }
        }
    }
    
    private void ActivateHandlingUnitsAndBarcodes()
    {
        for(int i = 0; i < handlingUnitTrackerList.Count; i++)
        {
            int storageLocationId = Convert.ToInt32(handlingUnitTrackerList[i].storageLocationId);

            int storageLocationUnitId = (storageLocationId - 1) / 15 + 1;//location.StorageLocationUnitId;

            GameObject locationUnit = aisle.transform.GetChild(storageLocationUnitId - 1).gameObject;

            GameObject palette = locationUnit.transform.GetChild(Convert.ToInt32((storageLocationId - 1) % 15)).gameObject;
            
            if(handlingUnitTrackerList[i].handlingUnitPalette != null)
            {
                palette.SetActive(true);

                for (int j = 0; j < handlingUnitTrackerList[i].handlingUnitBoxes.Count; j++)
                {
                    palette.transform.GetChild(j).gameObject.SetActive(true);
                    palette.transform.GetChild(j).GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>().text = handlingUnitTrackerList[i].handlingUnitBoxes[j].HandlingUnitCode;
                    palette.transform.GetChild(j).name = handlingUnitTrackerList[i].handlingUnitBoxes[j].HandlingUnitCode;
                }

                palette.transform.GetChild(16).GetChild(0).GetChild(0).GetComponent<Text>().text = handlingUnitTrackerList[i].handlingUnitPalette.HandlingUnitCode;
                palette.name = handlingUnitTrackerList[i].handlingUnitPalette.HandlingUnitCode;
            }
        }
    }

    private StorageLocation ReadStorageLocation(int storageLocationId)
    {
        StorageLocation location = storageLocations[storageLocationId - 1];
        return location;
    }

    private HandlingUnit ReadHandlingUnit(int handlingUnitId)
    {
        HandlingUnit unit = handlingUnits.Where(handlingUnit => handlingUnit.HandlingUnitId == handlingUnitId).FirstOrDefault();
        return unit;
    }

    private void ActivateStorageLocationBarcodes()
    {

        for(int i = 0; i < storageLocations.Count; i++)
        {
            GameObject locationUnit = aisle.transform.GetChild(i / 15).gameObject;
            GameObject locationBarcode = locationUnit.transform.GetChild(25 + i % 15).GetChild(0).GetChild(0).gameObject;
            locationBarcode.GetComponent<Text>().text = storageLocations[i].StorageLocationBarcode;

        }
    }

    private void ActivateStorageLocations()
    {
        int locationCount = storageLocations.Count / 15; //storageLocations[storageLocations.Count - 1].StorageLocationUnitId;

        for (int i = 0; i < locationCount; i++)
        {

            if(i < locationCount / 2)
            {
                GameObject obj = Instantiate(columnPrefab, new Vector3(0, 0, i * (-2.8f)), Quaternion.identity) as GameObject;
                obj.transform.SetParent(aisle.transform);
                obj.name = "Location_Unit_" + (i + 1).ToString();
            }
            else
            {
                Quaternion rotation = Quaternion.identity;
                rotation.eulerAngles = new Vector3(0f, 180f, 0f);
                GameObject obj = Instantiate(columnPrefab_1, new Vector3(-4.05f, 0, (i + 1 - locationCount / 2) * (-2.8f)), rotation) as GameObject;
                obj.transform.SetParent(aisle.transform);
                obj.name = "Location_Unit_" + (i + 1).ToString();
            }
        }
    }

    private void PostHandlingUnits()
    {
        RequestHeader header = new RequestHeader
        {
            Key = "Content-Type",
            Value = "application/json"
        };

        TextAsset handlingUnitData = Resources.Load<TextAsset>("HandlingUnit");

        string[] data = handlingUnitData.text.Split(new char[] { '\n' });

        int count = 0;

        for (int i = 1; i < data.Length - 1; i++) // first line is headings
        {
            string[] row = data[i].Split(new char[] { ',' });

            HandlingUnit handlingUnit = new HandlingUnit();

            handlingUnit.HandlingUnitId = long.Parse(row[0]);

            handlingUnit.HandlingUnitCode = row[1];

            int isCarrier = Convert.ToInt32(row[2]);

            if (isCarrier == 0)
                handlingUnit.HandlingUnitCarrier = false;
            else
                handlingUnit.HandlingUnitCarrier = true;

            string parentHandlingUnitId = row[3];

            if (parentHandlingUnitId != "NULL")
                handlingUnit.ParentHandlingUnitId = long.Parse(row[3]);

            handlingUnit.StorageLocationId = long.Parse(row[4]);

            StartCoroutine(RestWebClient.Instance.HttpPost($"{baseUrl}api/handlingunits", JsonConvert.SerializeObject(handlingUnit), (r) => OnRequestComplete(r), new List<RequestHeader> { header }));
        }
    }

    private void SetHandlingUnits()
    {
        TextAsset handlingUnitData = Resources.Load<TextAsset>("HandlingUnit");

        string[] data = handlingUnitData.text.Split(new char[] { '\n' });

        int count = 0;

        for (int i = 1; i < data.Length - 1; i++) // first line is headings
        {
            string[] row = data[i].Split(new char[] { ',' });

            HandlingUnit handlingUnit = new HandlingUnit();

            handlingUnit.HandlingUnitId = long.Parse(row[0]);

            handlingUnit.HandlingUnitCode = row[1];

            int isCarrier = Convert.ToInt32(row[2]);

            if (isCarrier == 0)
                handlingUnit.HandlingUnitCarrier = false;
            else
                handlingUnit.HandlingUnitCarrier = true;

            string parentHandlingUnitId = row[3];

            if (parentHandlingUnitId != "NULL")
                handlingUnit.ParentHandlingUnitId = long.Parse(row[3]);

            handlingUnit.StorageLocationId = long.Parse(row[4]);

            if (handlingUnitTrackerList.Where(handlingUnitTracker => handlingUnitTracker.storageLocationId == handlingUnit.StorageLocationId).FirstOrDefault() == null)
            {
                HandlingUnitTracker tracker = new HandlingUnitTracker(handlingUnit.StorageLocationId);

                if (handlingUnit.HandlingUnitCarrier)
                {
                    tracker.SetHandlingUnitPalette(handlingUnit);
                }
                else
                {
                    tracker.AddHandlingUnitBox(handlingUnit);
                    tracker.amount++;
                }

                handlingUnitTrackerList.Add(tracker);
            }
            else
            {
                HandlingUnitTracker tracker = handlingUnitTrackerList.Where(handlingUnitTracker => handlingUnitTracker.storageLocationId == handlingUnit.StorageLocationId).FirstOrDefault();

                if (handlingUnit.HandlingUnitCarrier)
                {
                    tracker.SetHandlingUnitPalette(handlingUnit);
                }
                else
                {
                    if (tracker.amount < 16)
                    {
                        tracker.AddHandlingUnitBox(handlingUnit);
                        tracker.amount++;
                    }
                }
            }
        }
    }

    private void SetStorageLocations()
    {
        TextAsset storageLocationData = Resources.Load<TextAsset>("StorageLocation");

        string[] data = storageLocationData.text.Split(new char[] { '\n' });

        for (int i = 1; i < data.Length - 1; i++) // first line is headings
        {
            string[] row = data[i].Split(new char[] { ',' });

            StorageLocation storagelocation = new StorageLocation();

            storagelocation.StorageLocationId = long.Parse(row[0]);

            storagelocation.StorageLocationBarcode = row[1];

            long pickingSequence = long.Parse(row[2]);
            storagelocation.PickingSequence = pickingSequence;

            int storageLocationRow = Convert.ToInt32(row[3]);
            storagelocation.StorageLocationRow = storageLocationRow;

            storagelocation.StorageLocationAisle = row[4];

            storageLocations.Add(storagelocation);
        }

        storageLocations = storageLocations.OrderBy(location => location.StorageLocationId).ToList();

        
       // for (int i = 0; i < storageLocations.Count; i++)
       // {
         //   storageLocations[i].StorageLocationUnitId = i / 15 + 1;  // storageLocations[i].StorageLocationUnitId => (storageLocations[i].StorageLocationId - 1) / 15 + 1
        //}
        
    }
    
    void OnRequestComplete(Response response)
    {
        string jsonString = response.Data;
        jsonString = jsonString.Replace(" ", String.Empty);

        storageLocations = JsonConvert.DeserializeObject<List<StorageLocation>>(jsonString);
        storageLocations = storageLocations.OrderBy(location => location.StorageLocationId).ToList();

        //foreach (StorageLocation sl in storageLocations)
         //   Debug.Log(sl.StorageLocationId);

        /*
        SetHandlingUnits();
        ActivateStorageLocations();
        ActivateHandlingUnitsAndBarcodes();
        CreateStorageLocationsButtons();
        CreateHandlingUnitsButtons();
        ActivateStorageLocationBarcodes();
        */
    }

    void OnTempUnitRequestComplete(Response response)
    {
        string jsonString = response.Data;
        jsonString = jsonString.Replace(" ", String.Empty);

        tempUnits = JsonConvert.DeserializeObject<List<TempUnit>>(jsonString);

        if(tempUnits.Count != 0)
        {
            HandlingUnit handlingUnit = handlingUnits.Where(hu => hu.HandlingUnitId == tempUnits[0].TempUnitId).FirstOrDefault();

            if (handlingUnit != null)
            {
                tested = true;
                CollectBox(handlingUnit);

            }
        }
    }

    void OnHandlingUnitRequestComplete(Response response)
    {
        string jsonString = response.Data;
        jsonString = jsonString.Replace(" ", String.Empty);

        handlingUnits = JsonConvert.DeserializeObject<List<HandlingUnit>>(jsonString);

        for(int i = 0; i < handlingUnits.Count; i++)
        {
            if (handlingUnitTrackerList.Where(handlingUnitTracker => handlingUnitTracker.storageLocationId == handlingUnits[i].StorageLocationId).FirstOrDefault() == null)
            {
                HandlingUnitTracker tracker = new HandlingUnitTracker(handlingUnits[i].StorageLocationId);

                if (handlingUnits[i].HandlingUnitCarrier)
                {
                    tracker.SetHandlingUnitPalette(handlingUnits[i]);
                }
                else
                {
                    tracker.AddHandlingUnitBox(handlingUnits[i]);
                    tracker.amount++;
                }

                handlingUnitTrackerList.Add(tracker);
            }
            else
            {
                HandlingUnitTracker tracker = handlingUnitTrackerList.Where(handlingUnitTracker => handlingUnitTracker.storageLocationId == handlingUnits[i].StorageLocationId).FirstOrDefault();

                if (handlingUnits[i].HandlingUnitCarrier)
                {
                    tracker.SetHandlingUnitPalette(handlingUnits[i]);
                }
                else
                {
                    if (tracker.amount < 16)
                    {
                        tracker.AddHandlingUnitBox(handlingUnits[i]);
                        tracker.amount++;
                    }
                }
            }
        }

        ActivateStorageLocations();
        ActivateHandlingUnitsAndBarcodes();
        CreateStorageLocationsButtons();
        CreateHandlingUnitsButtons();
        ActivateStorageLocationBarcodes();
    }
}
