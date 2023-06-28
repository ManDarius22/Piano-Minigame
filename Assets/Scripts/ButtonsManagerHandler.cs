using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using Unity.Mathematics;

using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class ColorForButtons
{
    public Color color;
}

public class ButtonsManagerHandler : MonoBehaviour
{
    private static int BaseColorChangeCount = 3;
    
    [SerializeField] private List<CustomButton> _buttonList = new List<CustomButton>();
    [SerializeField] private List<ColorForButtons> _buttonsColor = new List<ColorForButtons>();
    [SerializeField] private ScreenAnimations _screenAnimations;
    [SerializeField] private Button _continueButtonWinScreen;
    [SerializeField] private Button _continueButtonLoseScreen;

    private string dataFilePath;

    private List<int> _defaultValues = new List<int>();
    private List<int> _userValues = new List<int>();
    
    private bool _correctRound = false;
    private bool _nextRound = true;
    
    private int loadedLevelValue;

    private Coroutine _changeColorsCoroutineAfterRound;

    private void Start()
    {
        dataFilePath = Path.Combine(Application.dataPath, "playerLevel.json");
        LoadData();

        _changeColorsCoroutineAfterRound = StartCoroutine(ChangeButtonsColors());

        foreach (var button in _buttonList)
        {
            button.GetComponent<Button>().onClick.AddListener(GetButtonValueOnClick);
        }

        _continueButtonWinScreen.onClick.AddListener(ContinueButtonWinScreen_OnClick);
        _continueButtonLoseScreen.onClick.AddListener(ContinueButtonLoseScreen_OnClick);
    }

    private void OnDestroy()
    {
        foreach (var button in _buttonList)
        {
            button.GetComponent<Button>().onClick.RemoveListener(GetButtonValueOnClick);
        }

        _continueButtonWinScreen.onClick.RemoveListener(ContinueButtonWinScreen_OnClick);
        _continueButtonLoseScreen.onClick.RemoveListener(ContinueButtonLoseScreen_OnClick);
    }


    private IEnumerator ChangeButtonsColors()
    {
        while (true)
        {
            loadedLevelValue = LoadData();
            int changesPerformed = 0;

            yield return new WaitForSeconds(0.5f);
            Shuffle(_buttonList);

            while(changesPerformed < loadedLevelValue)
            {
                foreach (var button in _buttonList)
                {
                    button.GetComponent<Image>().color = _buttonsColor[UnityEngine.Random.Range(1, 5)].color;
                    _defaultValues.Add(button.buttonValue.value);
                    yield return new WaitForSeconds(0.5f);
                    button.GetComponent<Image>().color = Color.white;
                    yield return new WaitForSeconds(0.5f);
                    changesPerformed++;

                    if(changesPerformed >= loadedLevelValue)
                    {
                        break;
                    }
                }
            }

            for (int i = 0; i < _defaultValues.Count; i++)
            {
                Debug.LogError("First index: " + i + " is " + _defaultValues[i]);
            }

            if (_nextRound)
            {
                _nextRound = false;
                yield break;
            }
        }
    }

    private void Shuffle<T>(List<T> list)
    {
        int maxElements = list.Count;
        while (maxElements > 1)
        {
            maxElements--;
            int randomElement = UnityEngine.Random.Range(0, maxElements + 1);
            T temp = list[randomElement];
            list[randomElement] = list[maxElements];
            list[maxElements] = temp;
        }
    }

    private void StartNextRound()
    {
        _nextRound = true;

        _defaultValues.Clear();
        _userValues.Clear();

        if(_changeColorsCoroutineAfterRound != null)
        {
            StopCoroutine(_changeColorsCoroutineAfterRound);
        }

        _changeColorsCoroutineAfterRound = StartCoroutine(ChangeButtonsColors());
    }

    private void GetButtonValueOnClick()
    {
        CustomButton customButton = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<CustomButton>();

        if (customButton != null && customButton.buttonValue != null)
        {
            if (_userValues.Count < _defaultValues.Count)
                _userValues.Add(customButton.buttonValue.value);
            else
                return;
        }
    }

    public void UserInputCheck()
    {
        for (int i = 0; i < _userValues.Count; i++)
        {
            Debug.LogError("First index: " + i + " is " + _userValues[i]);
        }
    }

    public void ResultCheck()
    {
        if (_defaultValues.Count != _userValues.Count)
            return;

        for (int i = 0; i < _defaultValues.Count; i++)
        {
            if (_defaultValues[i] == _userValues[i])
                _correctRound = true;
            else
                _correctRound = false;
        }

        if (_correctRound == true)
        {
            Debug.LogError("Win");
            _screenAnimations.WinScreenAnimation_Show();
            SaveData(loadedLevelValue+1);
        }
        else
        {
            Debug.LogError("Lose");
            _screenAnimations.LoseScreenAnimation_Show();
            SaveData(3);
        }
    }

    private void ContinueButtonWinScreen_OnClick()
    {
        _screenAnimations.WinScreenAnimation_Hide(StartNextRound);
    }

    private void ContinueButtonLoseScreen_OnClick()
    {
        _screenAnimations.LoseScreenAnimation_Hide(StartNextRound);
    }

    public int LoadData()
    {
        if (File.Exists(dataFilePath))
        {
            string json = File.ReadAllText(dataFilePath);
            SaveLevel data = JsonUtility.FromJson<SaveLevel>(json);
            return data.levelValue;
        }
        else
        {
            CreateDefaultDataFile();
            return 0; // Default value if the file doesn't exist
        }
    }

    public void SaveData(int value)
    {
        SaveLevel data = new SaveLevel();
        data.levelValue = value;

        string json = JsonUtility.ToJson(data);
        File.WriteAllText(dataFilePath, json);
    }

    private void CreateDefaultDataFile()
    {
        SaveLevel defaultData = new SaveLevel();
        defaultData.levelValue = 3;

        string json = JsonUtility.ToJson(defaultData);
        File.WriteAllText(dataFilePath, json);
    }
}
