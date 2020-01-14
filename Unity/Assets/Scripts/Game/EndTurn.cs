using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EndTurn : MonoBehaviour
{
    public void OnClick()
    {
        APIWrapper.Instance.PlayerEndTurn((response, error) =>
        {
            if (error != null)
            {
                Debug.Log(error); // TODO: Show error to player
            }
        });
    }
}
