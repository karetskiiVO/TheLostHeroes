using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DescriberBehavour : MonoBehaviour { // TODO: сделать модифицируемый список кнопок спелов
    [SerializeField]private Text entityName;
    [SerializeField]private Text entityDescription;

    public interface IActionButton { // Содержит информацию о кнопке
        public string Text ();
        public void Callback ();
    }

    private class DebugActionButton : IActionButton {
        public void Callback () {}

        public string Text () {
            return "";
        }
    }

    public class Description {
        public string entityName;
        public string entityDescription;
        public IActionButton[] actionButtons;
    }

    private GameObject[] actionButtonGameObjects;
    private Button[] actionButtons;
    private Text[] actionButtonTexts;
    private IActionButton[] buttonsCallbacks;

    private void Start () {
        actionButtonGameObjects = new GameObject[] {
            GameObject.Find("Action1Button"),
            GameObject.Find("Action2Button"),
            GameObject.Find("Action3Button"),
        };

        actionButtons = new Button[] {
            actionButtonGameObjects[0].GetComponent<Button>(),
            actionButtonGameObjects[1].GetComponent<Button>(),
            actionButtonGameObjects[2].GetComponent<Button>(),
        };

        actionButtonTexts = new Text[] {
            actionButtonGameObjects[0].transform.GetComponentInChildren<Text>(),
            actionButtonGameObjects[1].transform.GetComponentInChildren<Text>(),
            actionButtonGameObjects[2].transform.GetComponentInChildren<Text>(),
        };

        SetDescription(new Description{
            entityName = "",
            entityDescription = "",
            actionButtons = new IActionButton[] {}
        });
    }

    public void SetDescription (Description description) {
        entityName.text = description.entityName;
        entityDescription.text = description.entityDescription;

        for (var i = 0; i < actionButtonGameObjects.Length; i++) {
            if (i < description.actionButtons.Length) {
                actionButtonGameObjects[i].SetActive(true);
                actionButtonTexts[i].text = description.actionButtons[i].Text();

                // подписываем действие на кнопку
                actionButtons[i].onClick.RemoveAllListeners();
                actionButtons[i].onClick.AddListener(description.actionButtons[i].Callback);
            } else {
                actionButtonGameObjects[i].SetActive(false);
            }
        }

        buttonsCallbacks = description.actionButtons;
    }
}
