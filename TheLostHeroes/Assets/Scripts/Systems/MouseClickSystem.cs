using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leopotam.Ecs;

using Photon.Pun;
using UnityEngine.UI;
public struct MouseClickSystem : IEcsRunSystem, IEcsInitSystem
{
    private EcsWorld ecsWorld;          // подтягивается автоматически, так как наследует EcsWorld
    private StaticData staticData;      // подтягивается из Inject

    private RuntimeData runtimeData;    // подтягивается из Inject

    private Button backgroundButton;
    private class BoolClass
    {
        public bool val = false;

        public void Activate () {
            val = true;
        }
    }
    private BoolClass clickedFlag;

    public void Init()
    {
        clickedFlag = new BoolClass();

        var backgroundButtonGameObject = GameObject.Find("Canvas");

        var img = backgroundButtonGameObject.AddComponent<Image>();
        img.color = new Color(1, 1, 1, 0); 
        var transform = backgroundButtonGameObject.GetComponent<RectTransform>();
        transform.sizeDelta = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);
        transform.anchoredPosition = Vector2.zero;
        backgroundButton = backgroundButtonGameObject.AddComponent<Button>();

        backgroundButton.onClick.AddListener(clickedFlag.Activate);
    }

    public void Run()
    {
        if (clickedFlag.val)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
            Clickable clicked = null;

            if (hit)
            {
                clicked = hit.collider.gameObject.GetComponent<Clickable>();
            }
            clicked ??= runtimeData.defaultClickableBehavour;

            clicked?.OnClick();
        }

        clickedFlag.val = false;
    }
}
