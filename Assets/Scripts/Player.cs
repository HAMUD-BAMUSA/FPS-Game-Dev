using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;


public class Player : MonoBehaviour
{
    public int HP = 100;
    public GameObject bloodyScreen;

    public TextMeshProUGUI playerHealthUI;
    public GameObject GameOverUI;

    public bool isDead;

    // display health at game start and each time its updated
    private void Start()
    {
        playerHealthUI.text = $"Health: {HP}";
    }

    [SerializeField] private FirstPersonController fpsController;
    [SerializeField] private Animator cameraAnimator;

    [SerializeField] private AudioSource playerPainAudio;
    [SerializeField] private AudioSource playerDeathAudio;

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        HP -= amount;

        if (playerPainAudio != null)
        {
            playerPainAudio.Play();
        }

        if (HP <= 0)
        {
            Debug.Log("Player Dead");
            isDead = true;
            PlayerDead();
        }
        else
        {
            Debug.Log("Player Hit");
            StartCoroutine(BloodyScreenEffect());
            playerHealthUI.text = $"Health: {HP}";
        }
    }

    private void PlayerDead()
    {
        if (fpsController != null)
            fpsController.enabled = false;

            //fix cursor after death so its visible
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

        // Play death sound
        if (playerDeathAudio != null)
            playerDeathAudio.Play();
        else
            Debug.LogError("Player death audio not assigned!");

        if (cameraAnimator != null)
        {
            cameraAnimator.enabled = true;
            playerHealthUI.gameObject.SetActive(false);
        }
        else
            Debug.LogError("Camera Animator not assigned!");

        GetComponent<ScreenFader>().StartFade();
        StartCoroutine(ShowGameOverUI());
    }

    private IEnumerator ShowGameOverUI()
    {
        yield return new WaitForSeconds(1f);

        GameOverUI.gameObject.SetActive(true);

        // waves survived = current wave - 1
        int wavesSurvived = ZombieSpawnController.Instance.CurrentWave - 1;

        SaveLoadManager.Instance.SaveHighScore(wavesSurvived);

        StartCoroutine(ReturnToMainMenu());
    }

    private IEnumerator ReturnToMainMenu()
    {
        yield return new WaitForSeconds(5f);

        SceneManager.LoadScene("MainMenu");
    }

    private IEnumerator BloodyScreenEffect()
    {
        if (bloodyScreen.activeInHierarchy == false)
        {
            bloodyScreen.SetActive(true);
        }

        // delay for the animation and for blood fading
        var image = bloodyScreen.GetComponentInChildren<Image>();

        // Set the initial alpha value to 1 (fully visible).
        Color startColor = image.color;
        startColor.a = 1f;
        image.color = startColor;

        float duration = 2f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            // Calculate the new alpha value using Lerp (Linear Interpolation)
            //used for moving or changing values over a period of time.
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / duration);

            // Update the color with the new alpha value (opacity)
            Color newColor = image.color;
            newColor.a = alpha;
            image.color = newColor;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (bloodyScreen.activeInHierarchy)
        {
            bloodyScreen.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ZombieHand"))
        {
            if (isDead == false) // avoid dying under 0 health loop
            {
                TakeDamage(other.gameObject.GetComponent<ZombieHand>().damage);
            }
        }
    }
}
