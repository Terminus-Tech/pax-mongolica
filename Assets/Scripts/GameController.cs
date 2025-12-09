using TMPro;
using Unity.VisualScripting;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public GameObject player;
    public GameObject canvas;
    public AudioSource audioSource;
    public AudioClip key; 
    public AudioClip good;
    public AudioClip bad;


    int dialogIndex = 0;
    string[] textlist = new string[] { };
    string encounterType = "";
    bool dialogProgress = false;
    bool dialogActive = false;

    public int friendlyRations = 5;
    public int hostileRations = 5;
    public int terrainRations = 5;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Encounter("menu", new string[] { "Press any key to begin" });
    }

    private void FixedUpdate()
    {
        if (dialogActive)
        {
            if (dialogIndex < textlist.Length)
            {
                if (!dialogProgress && Keyboard.current.anyKey.IsActuated())
                {
                    canvas.transform.GetChild(8).gameObject.GetComponent<TextMeshProUGUI>().text = textlist[dialogIndex];
                    dialogIndex++;
                    dialogProgress = true;
                } else if (dialogProgress && !Keyboard.current.anyKey.IsActuated())
                {
                    dialogProgress = false;
                }
            }
            else
            {
                switch (encounterType)
                {
                    case "menu":
                        canvas.transform.GetChild(0).gameObject.SetActive(false);

                        canvas.transform.GetChild(6).gameObject.SetActive(true);
                        canvas.transform.GetChild(7).gameObject.SetActive(true);
                        break;
                    case "friendly":
                        canvas.transform.GetChild(1).gameObject.SetActive(false);
                        player.GetComponent<PlayerController>().rations += friendlyRations;
                        audioSource.PlayOneShot(key);
                        break;
                    case "hostile":
                        canvas.transform.GetChild(2).gameObject.SetActive(false);
                        player.GetComponent<PlayerController>().rations -= hostileRations;
                        break;
                    case "terrain":
                        canvas.transform.GetChild(3).gameObject.SetActive(false);
                        player.GetComponent<PlayerController>().rations -= terrainRations;
                        break;
                    case "win":
                        SceneManager.LoadScene("MainScene");
                        break;
                    case "lose":
                        SceneManager.LoadScene("MainScene");
                        break;
                }

                canvas.transform.GetChild(8).gameObject.GetComponent<TextMeshProUGUI>().text = "";

                player.GetComponent<PlayerController>().moveable = true;
                dialogActive = false;
                Debug.Log("Encounter ended.");
            }
        }
        canvas.transform.GetChild(7).gameObject.GetComponent<TextMeshProUGUI>().text = $"x {player.GetComponent<PlayerController>().rations}";
    }

    public void Encounter(string type, string[] text)
    {
        player.GetComponent<PlayerController>().moveable = false;
        dialogIndex = 0;
        dialogProgress = true;
        encounterType = type;
        textlist = text;

        switch (encounterType)
        {
            case "menu":
                canvas.transform.GetChild(0).gameObject.SetActive(true);
                break;
            case "friendly":
                canvas.transform.GetChild(1).gameObject.SetActive(true);
                break;
            case "hostile":
                canvas.transform.GetChild(2).gameObject.SetActive(true);
                audioSource.PlayOneShot(bad);
                break;
            case "terrain":
                canvas.transform.GetChild(3).gameObject.SetActive(true);
                audioSource.PlayOneShot(bad);
                break;
            case "win":
                canvas.transform.GetChild(4).gameObject.SetActive(true);
                audioSource.PlayOneShot(good);
                break;
            case "lose":
                canvas.transform.GetChild(5).gameObject.SetActive(true);
                audioSource.PlayOneShot(bad);
                break;
        }

        canvas.transform.GetChild(8).gameObject.GetComponent<TextMeshProUGUI>().text = textlist[0];
        dialogActive = true;
        Debug.Log("Encounter started: " + encounterType);
    }
}
