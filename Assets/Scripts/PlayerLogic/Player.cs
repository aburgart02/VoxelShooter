using System.Collections.Generic;
using Infrastructure.Factory;
using Infrastructure.Services.Input;
using Inventory;
using Mirror;
using Networking;
using TMPro;
using UI;
using UnityEngine;

namespace PlayerLogic
{
    public class Player : NetworkBehaviour
    {
        public InventoryInput InventoryInput { get; private set; }
        public List<int> ItemsIds { get; private set; }
        public float PlaceDistance { get; private set; }

        [SerializeField]
        private TextMeshProUGUI nickNameText;

        [SerializeField]
        private Transform itemPosition;

        public Transform ItemPosition => itemPosition;

        [SerializeField]
        private MeshRenderer[] bodyParts;

        [SerializeField]
        private GameObject nickNameCanvas;

        [SerializeField]
        private Transform bodyOrientation;

        [SerializeField]
        private Transform headPivot;

        [SerializeField]
        private Transform cameraMountPoint;

        [SerializeField]
        private CapsuleCollider hitBox;

        [SerializeField]
        private new Rigidbody rigidbody;

        private Hud _hud;

        private IInputService _inputService;

        private PlayerMovement _movement;
        private PlayerRotation _rotation;

        public void Construct(IClient client, IUIFactory uiFactory, IInputService inputService, float placeDistance,
            List<int> itemIds, float speed, float jumpHeight)
        {
            PlaceDistance = placeDistance;
            ItemsIds = itemIds;
            _inputService = inputService;
            InventoryInput = new InventoryInput(_inputService);
            _movement = new PlayerMovement(hitBox, rigidbody, bodyOrientation, speed, jumpHeight);
            _rotation = new PlayerRotation(bodyOrientation, headPivot);
            _hud = uiFactory.CreateHud(this, client, inputService);
            TurnOffNickName();
            TurnOffBodyRender();
            MountCamera();
        }

        private void Update()
        {
            if (!isLocalPlayer)
            {
                return;
            }

            _movement.Move(_inputService.Axis);

            if (_inputService.IsJumpButtonDown())
            {
                _movement.Jump();
            }

            _rotation.Rotate(_inputService.MouseAxis);
            InventoryInput.Update();
        }

        private void FixedUpdate()
        {
            _movement.FixedUpdate();
        }

        public void SetNickName(string nickName)
        {
            nickNameText.SetText(nickName);
        }

        private void TurnOffBodyRender()
        {
            for (var i = 0; i < bodyParts.Length; i++)
            {
                bodyParts[i].GetComponent<MeshRenderer>().enabled = false;
            }
        }

        private void TurnOffNickName()
        {
            nickNameCanvas.SetActive(false);
        }

        private void MountCamera()
        {
            var cameraTransform = Camera.main.gameObject.transform;
            cameraTransform.parent = cameraMountPoint.transform;
            cameraTransform.position = cameraMountPoint.position;
            cameraTransform.rotation = cameraMountPoint.rotation;
            Camera.main.fieldOfView = Constants.DefaultFov;
        }

        private void OnDestroy()
        {
            Destroy(_hud.gameObject);
        }
    }
}