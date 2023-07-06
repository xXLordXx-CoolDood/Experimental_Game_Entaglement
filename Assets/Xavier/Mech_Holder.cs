using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Mech_Holder : MonoBehaviour
{
    [SerializeField] private GameObject mech;
    [SerializeField] private TextMeshProUGUI scoreText;

    private int points = 0;

    public void MechDie(GameObject debris)
    {
        StartCoroutine(RespawnTimer(debris));
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Points")
        {
            points += other.GetComponent<Score>().value;
            scoreText.text = $"Score - {points}pts";
            Destroy(other.gameObject);
        }
    }

    IEnumerator RespawnTimer(GameObject debris)
    {
        mech.SetActive(false);
        yield return new WaitForSeconds(5);
        mech.SetActive(true);
        mech.GetComponent<Mech_Controller>().Respawn();
        Destroy(debris);
    }
}
