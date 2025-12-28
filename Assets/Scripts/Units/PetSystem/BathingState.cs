using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BathingState : PetBaseState
{
    private List<GameObject> bubbles = new List<GameObject>();
    private float timeBetweenBubbles = 0.1f;
    private float bubbleScaleRate = 0.2f;

    private float timer;
    private bool isSoaping = false;
    private bool isShowAllBubbles = false;
    private bool isHidingAllBubbles = false;

    public bool IsShowAllBubbles => isShowAllBubbles;
    public bool IsHidingAllBubbles => isHidingAllBubbles;

    public override void EnterState(PetStateManager cat)
    {
        StatsRate = cat.PetRateDict[PetStateType.WalkAround];
        bubbles = cat.Bubbles.ToList();
        CreateRandomList(bubbles);
        
        timer = timeBetweenBubbles;

        cat.Animator.SetFloat("Vert", 0);
    }

    public override void UpdateState(PetStateManager cat)
    {
        if (isSoaping)
            return;

        StatsRate = cat.PetRateDict[PetStateType.WalkAround];
        base.UpdateState(cat);
    }

    public override void OnInteract(PetStateManager cat)
    {
        if (!isSoaping)
        {
            cat.ChangeState(cat.beingPickUpState);
        }
    }

    public void OnShowerInteract(PetStateManager cat)
    {
        if (!isSoaping)
            return;

        StatsRate = cat.PetRateDict[PetStateType.Bath];
        base.UpdateState(cat);

        if (!isHidingAllBubbles)
        {
            HandleHidingBubbles(cat);
        }
    }


    public void OnSoapInteract(PetStateManager cat)
    {
        isSoaping = true;

        StatsRate = cat.PetRateDict[PetStateType.Bath];
        base.UpdateState(cat);

        if (!isShowAllBubbles)
        {
            HandleShowingBubbles(cat);
        }
    }

    public override void ExitState(PetStateManager cat)
    {
        ToolHandler toolHandler = GameManager.Instance.Player.GetComponent<ToolHandler>();
        BathTub bathTub = toolHandler.CurrentInteraction.GetComponent<BathTub>();

        if (bathTub != null)
        {
            bathTub.OnPetExitBathTub();
        }
    }

    private void HandleShowingBubbles(PetStateManager cat)
    {
        timer -= Time.deltaTime;

        if (timer < 0)
        {
            isShowAllBubbles = true;
            foreach (GameObject bubble in bubbles)
            {
                if (!bubble.activeInHierarchy)
                {
                    bubble.SetActive(true);
                    isShowAllBubbles = false;
                    break;
                }
            }

            timer = timeBetweenBubbles;
        }
    }

    private void HandleHidingBubbles(PetStateManager cat)
    {
        timer -= Time.deltaTime;

        if (timer < 0)
        {
            isHidingAllBubbles = true;
            foreach (GameObject bubble in bubbles)
            {
                if (bubble.activeInHierarchy)
                {
                    bubble.SetActive(false);
                    isHidingAllBubbles = false;
                    break;
                }
            }

            if (isHidingAllBubbles)
            {
                isSoaping = false;
            }

            timer = timeBetweenBubbles;
        }
    }

    private void CreateRandomList(List<GameObject> list)
    {
        // Fisher-Yates shuffle algorithm
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            GameObject temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }
}
