using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Collections/List/InteractorList")]
public class InteractorListSO : ListSO<Interactor> {
    public void OnDisable()
    {
        Clear();
    }
}
