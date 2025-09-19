using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuestManager : MonoBehaviour
{
    public Transform questPanel;
    public GameObject textPrefab;
    public ParticleSystem confettis;

    public AudioClip ValidationSound;
    public AudioClip victorySound;

    private List<TextMeshProUGUI> quests = new List<TextMeshProUGUI>();
    private int currentQuestIndex = 0;

    void Start()
    {
        AddQuest("Prend le cube");
        AddQuest("Pivote le cube");
        AddQuest("Jette le cube");
    }

    public void AddQuest(string description)
    {
        GameObject obj = Instantiate(textPrefab, questPanel);
        TextMeshProUGUI tmp = obj.GetComponent<TextMeshProUGUI>();
        tmp.text = "- " + description;
        quests.Add(tmp);
    }

    // Appelle cette fonction quand le joueur effectue une action
    public void PlayerDidAction(string action)
    {
        if (currentQuestIndex >= quests.Count) return;

        string questText = quests[currentQuestIndex].text.Replace("- ", "").Replace("<s>", "").Replace("</s>", "");

        // Si l'action correspond à la quête courante
        if ((currentQuestIndex == 0 && action == "TakeCube") ||
            (currentQuestIndex == 1 && action == "RotateCube") ||
            (currentQuestIndex == 2 && action == "ThrowCube"))
        {
            CompleteQuest(currentQuestIndex);
            AudioSource.PlayClipAtPoint(ValidationSound, Camera.main.transform.position);
            currentQuestIndex++;
        }
    }

    private void CompleteQuest(int index)
    {
        if (index < 0 || index >= quests.Count) return;

        TextMeshProUGUI tmp = quests[index];
        tmp.text = "<s>" + tmp.text + "</s>"; // barre le texte
        CheckAllCompleted();
    }

   private void CheckAllCompleted()
{
    foreach (TextMeshProUGUI t in quests)
    {
        if (!t.text.Contains("<s>")) return; // pas encore terminé
    }

    if (confettis != null)
    {
        confettis.gameObject.SetActive(true); // active le parent si besoin

        // joue tous les Particle Systems enfants
        ParticleSystem[] systems = confettis.GetComponentsInChildren<ParticleSystem>();
        foreach (var ps in systems)
        {
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            ps.Play(true);
        }
    }

    AudioSource.PlayClipAtPoint(victorySound, Camera.main.transform.position);
}

}
