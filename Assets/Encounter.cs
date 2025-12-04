using Unity.VisualScripting;
using UnityEngine;

public class Encounter : MonoBehaviour
{
    public GameObject gameController;

    public string encounterType;
    public string[] dialogLines = { "" };

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            gameController.GetComponent<GameController>().Encounter(encounterType, dialogLines);
        }
    }
}
