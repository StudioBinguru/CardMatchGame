using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeButtonScale : MonoBehaviour
{
    [SerializeField] private float minScale = 0.9f;
    [SerializeField] private List<Transform> extraTarget;
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    public void OnClickUp()
    {
        if (button.IsInteractable())
        {
            SetScale(new Vector3(1, 1, 1)); // ���� ũ��� ����
        }
    }

    public void OnClickDown()
    {
        if (button.IsInteractable())
        {
            SetScale(new Vector3(minScale, minScale, minScale)); // ũ�� ���
        }
    }

    private void SetScale(Vector3 scale)
    {
        transform.localScale = scale;
        if (extraTarget.Count != 0)
        {
            foreach (Transform target in extraTarget)
            {
                target.localScale = scale;
            }
        }
    }

}
