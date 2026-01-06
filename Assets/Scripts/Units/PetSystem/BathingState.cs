using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BathingState : PetBaseState
{
    private List<GameObject> bubbles = new List<GameObject>();
    private float timeBetweenBubbles = 0.05f;

    private float timer;
    private bool isSoaping = false;
    private bool isShowAllBubbles = false;
    private bool isHidingAllBubbles = false;
    private bool isFirstTime = true;

    public bool IsShowAllBubbles => isShowAllBubbles;
    public bool IsHidingAllBubbles => isHidingAllBubbles;

    public override void EnterState(PetStateHandler cat)
    {
        StatsRate = cat.PetRateDict[PetStateType.WalkAround];
        bubbles = cat.Bubbles.ToList();
        CreateRandomList(bubbles);
        
        timer = timeBetweenBubbles;
        isFirstTime = true;

        cat.Animator.SetFloat("Vert", 0);
    }

    public override void UpdateState(PetStateHandler cat)
    {
        if (isSoaping)
            return;

        StatsRate = cat.PetRateDict[PetStateType.WalkAround];
        base.UpdateState(cat);
    }

    public override void OnInteract(PetStateHandler cat)
    {
        if (!isSoaping)
        {
            cat.ChangeState(cat.beingPickUpState);
        }
    }

    public void OnShowerInteract(PetStateHandler cat)
    {
        if (!isSoaping)
            return;

        StatsRate = cat.PetRateDict[PetStateType.Bathing];
        base.UpdateState(cat);

        if (!isHidingAllBubbles)
        {
            HandleHidingBubbles(cat);
        }
    }


    public void OnSoapInteract(PetStateHandler cat)
    {
        isSoaping = true;

        StatsRate = cat.PetRateDict[PetStateType.Bathing];
        base.UpdateState(cat);

        if (!isShowAllBubbles)
        {
            HandleShowingBubbles(cat);
        }
    }

    public override void ExitState(PetStateHandler cat)
    {
        ToolHandler toolHandler = GameManager.Instance.Player.GetComponent<ToolHandler>();
        BathTub bathTub = toolHandler.CurrentInteraction.GetComponent<BathTub>();

        if (bathTub != null)
        {
            bathTub.OnPetExitBathTub();
        }
    }

    private void HandleShowingBubbles(PetStateHandler cat)
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
                    isHidingAllBubbles = false;
                    break;
                }
            }

            if (isShowAllBubbles && isFirstTime)
            {
                GameManager.Instance.AddHeart(1);
                isFirstTime = false;
            }

            timer = timeBetweenBubbles;
        }
    }

    private void HandleHidingBubbles(PetStateHandler cat)
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
                    isShowAllBubbles = false;
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
