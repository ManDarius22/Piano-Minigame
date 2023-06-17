using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class ColorForButtons
{
    public Color color;
}

public class ButtonsManagerHandler : MonoBehaviour
{
    [SerializeField] private List<CustomButton> _buttonList = new List<CustomButton>();
    [SerializeField] private List<ColorForButtons> _buttonsColor = new List<ColorForButtons>();
    [SerializeField] private ScreenAnimations _screenAnimations;

    private List<int> _defaultValues = new List<int>();
    private List<int> _userValues = new List<int>();
    private bool _correctRound = false;

    private void Start()
    {
        StartCoroutine(ChangeButtonsColors());

        foreach (var button in _buttonList)
        {
            button.GetComponent<Button>().onClick.AddListener(GetButtonValueOnClick);
        }
    }

    private void OnDestroy()
    {
        foreach (var button in _buttonList)
        {
            button.GetComponent<Button>().onClick.RemoveListener(GetButtonValueOnClick);
        }
    }


    private IEnumerator ChangeButtonsColors()
    {
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < _buttonList.Count; i++)
        {
            int random = UnityEngine.Random.Range(0, _buttonList.Count);

            _buttonList[random].GetComponent<Image>().color = _buttonsColor[UnityEngine.Random.Range(1, 5)].color;
            _defaultValues.Add(_buttonList[random].buttonValue.value);

            yield return new WaitForSeconds(0.5f);
            _buttonList[random].GetComponent<Image>().color = Color.white;
            yield return new WaitForSeconds(0.5f);
        }

        for (int i = 0; i < _defaultValues.Count; i++)
        {
            Debug.LogError("Primul index: " + i + " este " + _defaultValues[i]);
        }
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

    public void VerificareUserInput()
    {
        for (int i = 0; i < _userValues.Count; i++)
        {
            Debug.LogError("Primul index: " + i + " este " + _userValues[i]);
        }
    }

    public void VerificareRezultate()
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

        if(_correctRound == true)
        {
            Debug.LogError("Castigat");
            _screenAnimations.WinScreenAnimation_Show();
        }
    }
}
