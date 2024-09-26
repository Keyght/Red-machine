using Camera;
using Connection;
using Events;
using Player;
using UnityEngine;
using Utils.Singleton;

public class CameraSwipeMovement : DontDestroyMonoBehaviourSingleton<CameraSwipeMovement>

{
    [Header("Parameters")]
    [SerializeField] private float _swipeSensitivity = 0.1f;
    [SerializeField] private float _smoothTime = 0.2f;
    [SerializeField] private Vector2 _minPosition;
    [SerializeField] private Vector2 _maxPosition;

    private Vector3 _velocity = Vector3.zero;
    private Vector3 _targetPosition;
    private Vector2 _lastSwipePosition;

    private void Start()
    {
        _targetPosition = transform.position;
    }

    public void SetupPositions(Transform[] nodeTransforms)
    {
        foreach (Transform node in nodeTransforms) 
        {
            if (node.position.x < _minPosition.x)
                _minPosition.x = node.position.x;
            else if (node.position.x > _maxPosition.x)
                _maxPosition.x = node.position.x;
            if (node.position.y < _minPosition.y)
                _minPosition.y = node.position.y;
            else if (node.position.y > _maxPosition.y)
                _maxPosition.y = node.position.y;
        }
    }

    private void Update()
    {
        if (PlayerController.PlayerState != PlayerState.None && PlayerController.PlayerState != PlayerState.Scrolling)
            return;

        HandleSwipe();
        SmoothMove();
    }

    private void HandleSwipe()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _lastSwipePosition = Input.mousePosition;
            EventsController.Fire(new EventModels.Game.Scrolled());
        }
        else if (Input.GetMouseButton(0) && PlayerController.PlayerState == PlayerState.Scrolling)
        {
            Vector2 swipeDelta = (Vector2)Input.mousePosition - _lastSwipePosition;
            Vector3 moveDelta = new Vector3(-swipeDelta.x, -swipeDelta.y, 0) * _swipeSensitivity;
            _targetPosition += moveDelta;

            _targetPosition.x = Mathf.Clamp(_targetPosition.x, _minPosition.x, _maxPosition.x);
            _targetPosition.y = Mathf.Clamp(_targetPosition.y, _minPosition.y, _maxPosition.y);

            _lastSwipePosition = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            EventsController.Fire(new EventModels.Game.PlayerFingerRemoved());
        }
    }

    private void SmoothMove()
    {
        transform.position = Vector3.SmoothDamp(transform.position, _targetPosition, ref _velocity, _smoothTime);
    }
}