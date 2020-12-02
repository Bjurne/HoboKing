using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "My Assets/CardData")]
public class CardData : ScriptableObject
{
    public Sprite cardSprite = default;
    public string[] cardDescriptions = default;
    public string cardName = "Card Name";
    public AbilityStep[] playedAbilitySteps;
    public AbilityStep[] targetedAbilitySteps;
    public bool reuseTarget = false;

    internal CardAbility cardAbility = default;
    private List<ICardAbilityStep> playedSteps;
    private List<ICardAbilityStep> targetedSteps;

    public void SetupData(Card card)
    {
        playedSteps = new List<ICardAbilityStep>();
        targetedSteps = new List<ICardAbilityStep>();

        foreach (AbilityStep step in playedAbilitySteps)
            CreateAbilityStep(step, playedSteps, card);
        playedSteps.Add(new CardFinishedResolving(AbilityStep.CardFinishedResolving, card));

        if (targetedAbilitySteps.Length > 0)
        {
            foreach (AbilityStep step in targetedAbilitySteps)
                CreateAbilityStep(step, targetedSteps, card);
            targetedSteps.Add(new CardFinishedResolving(AbilityStep.CardFinishedResolving, card));
        }

        cardAbility = new CardAbility(playedSteps, targetedSteps);
    }

    private void CreateAbilityStep(AbilityStep step, List<ICardAbilityStep> steps, Card card)
    {
        switch (step)
        {
            case AbilityStep.ChooseTarget:
                break;
            case AbilityStep.AddMarker:
                steps.Add(new AddMarker(step, GameLocalization.Instance.GetMarkerPrefab(), reuseTarget, card));
                break;
            case AbilityStep.RemoveMarker:
                steps.Add(new RemoveMarker(step, reuseTarget));
                break;
            case AbilityStep.CardFinishedResolving:
                break;
            case AbilityStep.BreachBarriers:
                steps.Add(new BreachBarriers(step, reuseTarget));
                break;
            case AbilityStep.AddHalfMarker:
                var fullMarkerPrefab = GameLocalization.Instance.GetMarkerPrefab();
                var halfMarkerPrefab = GameLocalization.Instance.CheatGetPrefab(false);
                steps.Add(new AddHalfMarker(step, halfMarkerPrefab, fullMarkerPrefab));
                break;
            case AbilityStep.Idle:
                break;
            default:
                break;
        }
    }
}
