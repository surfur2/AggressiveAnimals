using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[System.Serializable]
public class AnimalDied : UnityEvent<GameObject>
{

}

[System.Serializable]
public class AnimalHealthChange : UnityEvent<int, float>
{

}

[System.Serializable]
public class ResourceDepleted : UnityEvent<GameObject>
{

}

[System.Serializable]
public class PlayerEnterObstacle : UnityEvent<int, GameObject>
{

}

[System.Serializable]
public class PlayerExitObstacle : UnityEvent<int, GameObject>
{

}
