using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GoalManager : MonoBehaviour
{
    [SerializeField] private Transform _ballReset, _playerReset, _enemyReset;
    [SerializeField] private GameObject _player, _enemy, _ball;

    [SerializeField] private PlayerMovementStats _playerMovementStats;
    [SerializeField] private AIMovementStats _enemyMovementStats;

    [SerializeField] private float _timeBetweenGoals = 1;
    
    [SerializeField] private BoxCollider2D _goal1, _goal2;

    private void Start()
    {
        _ball.transform.position = _ballReset.transform.position;
        _player.transform.position = _playerReset.transform.position;
        _enemy.transform.position = _enemyReset.transform.position;
    }

    private void Update()
    {
        ResetBall();    
    }

    public void ResetBall()
    {
        if (PlayerGoalCounter.Instance.CurrentPlayerHealthCounter <= 0 ||
            EnemyGoalCounter.Instance.CurrentEnemyHealthCounter <= 0)
        {
            _goal1.enabled = false;
            _goal2.enabled = false;
            return;
        }
        if(PlayerGoalCounter.Instance.IsGoal == true || EnemyGoalCounter.Instance.IsGoal == true)
        {
            StartCoroutine(GoalProcedure());
            StartCoroutine(EnableEnemy(_enemy));
            StartCoroutine(Unfreeze(_enemy.GetComponent<Rigidbody2D>()));
            StartCoroutine(EnableEnemy(_player));
            StartCoroutine(Unfreeze(_player.GetComponent<Rigidbody2D>()));
        }
    }

    public IEnumerator GoalProcedure()
    {
        PlayerGoalCounter.Instance.IsGoal = false;
        EnemyGoalCounter.Instance.IsGoal = false;
        EnemyHealth.Instance.ResetHealth();
        PlayerHealth.Instance.ResetHealth();
        PowerCharacter2.CanSlow = false;
        PowerCharacter7.CanSlow = false;
        PowerCharacter3.IsPowering = false;
        PowerCharacter8.IsPowering = false;
        PowerCharacter4.CanStun = false;
        PowerCharacter9.CanStun = false;
        PowerCharacter5.CanDisable = false;
        PowerCharacter10.CanDisable = false;
        BallPhysics.Instance.Power2CharacterVFX.SetActive(false);
        BallPhysics.Instance.Power7CharacterVFX.SetActive(false);
        BallPhysics.Instance.Power3CharacterVFX.SetActive(false);
        BallPhysics.Instance.Power8CharacterVFX.SetActive(false);
        BallPhysics.Instance.Power4CharacterVFX.SetActive(false);
        BallPhysics.Instance.Power9CharacterVFX.SetActive(false);
        BallPhysics.Instance.Power5CharacterVFX.SetActive(false);
        BallPhysics.Instance.Power10CharacterVFX.SetActive(false);
        _playerMovementStats.MaxWalkSpeed = BallPhysics.Instance.OriginalWalkSpeed;
        _playerMovementStats.MaxRunSpeed = BallPhysics.Instance.OriginalRunSpeed;
        _enemyMovementStats.MaxWalkSpeed = BallPhysics.Instance.OriginalWalkSpeed;
        _enemyMovementStats.MaxRunSpeed = BallPhysics.Instance.OriginalRunSpeed;
        _ball.SetActive(false);
        _ball.transform.position = _ballReset.transform.position;
        _player.transform.position = _playerReset.transform.position;
        _enemy.transform.position = _enemyReset.transform.position;
        yield return new WaitForSeconds(_timeBetweenGoals);
        _ball.SetActive(true);
    }

    IEnumerator Unfreeze(Rigidbody2D enemy)
    {
        yield return new WaitForSeconds(_timeBetweenGoals);
        enemy.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    IEnumerator EnableEnemy(GameObject enemy)
    {
        yield return new WaitForSeconds(_timeBetweenGoals);
        enemy.SetActive(true);
    }
}
