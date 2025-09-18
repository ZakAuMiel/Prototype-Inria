using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuestManager : MonoBehaviour
{
    [Header("UI References")]
    public Transform questPanel;        // Le panel qui contiendra les quêtes
    public GameObject textPrefab;       // Prefab d’un TextMeshProUGUI
    public ParticleSystem confettis;    // Effet de fin

    private readonly List<GameObject> quests = new List<GameObject>();

    void Start()
    {
        // Exemple d’objectifs au lancement
        AddQuest("- Prends le cube");
        AddQuest("- Pivot le cube");
        AddQuest("- jette le cube");
    }

    /// <summary>
    /// Crée une nouvelle ligne de quête avec le texte donné.
    /// </summary>
    public void AddQuest(string description)
    {
        if (textPrefab == null || questPanel == null)
        {
            Debug.LogError("QuestManager : textPrefab ou questPanel n’est pas assigné dans l’inspecteur.");
            return;
        }

        GameObject obj = Instantiate(textPrefab, questPanel);
        TextMeshProUGUI tmp = obj.GetComponent<TextMeshProUGUI>();
        if (tmp != null)
        {
            tmp.text = description;
        }
        else
        {
            Debug.LogError("QuestManager : le prefab n’a pas de composant TextMeshProUGUI !");
        }

        quests.Add(obj);
    }

    /// <summary>
    /// Marque une quête comme complétée et vérifie si toutes sont terminées.
    /// </summary>
    public void CompleteQuest(int index)
    {
        if (index < 0 || index >= quests.Count) return;

        TextMeshProUGUI tmp = quests[index].GetComponent<TextMeshProUGUI>();
        if (tmp != null)
        {
            tmp.color = Color.green;
        }

        CheckAllCompleted();
    }

    private void CheckAllCompleted()
    {
        foreach (GameObject q in quests)
        {
            TextMeshProUGUI tmp = q.GetComponent<TextMeshProUGUI>();
            if (tmp == null || tmp.color != Color.green)
                return;
        }

        // Toutes les quêtes sont vertes
        if (confettis != null)
        {
            confettis.Play();
        }

        Debug.Log("Toutes les quêtes sont terminées !");
    }
}
