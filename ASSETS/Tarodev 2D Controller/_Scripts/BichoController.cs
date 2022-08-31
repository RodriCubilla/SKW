using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BichoController : MonoBehaviour
{
    private IMovement currentMovement;
    private BeastMovement beastMovement;
    private YoMovement yoMovement;
    bool isTransfomating;

    private void Start()
    {
        beastMovement = new BeastMovement();
        yoMovement = new YoMovement();

        currentMovement = beastMovement;
    }

    private void Update() {
        if (Input.GetKey(KeyCode.D)) {
            currentMovement.Walk();
        }
    }

    private IEnumerator Transformation() 
    {
        if (isTransfomating) yield return 0;

        if (currentMovement == beastMovement) {
            isTransfomating = true;
            yield return new WaitForSeconds(2f);
            currentMovement = yoMovement;
            isTransfomating = false;
        }else {
            isTransfomating = true;
            yield return new WaitForSeconds(2f);
            currentMovement = beastMovement;
            isTransfomating = false;
        }
    }
}