using System;
using Data;
using Infrastructure.Factory;
using Infrastructure.Services;
using Mirror;
using UnityEngine;

namespace PlayerLogic
{
    public class Player : NetworkBehaviour
    {
        public event Action<int> OnHealthChanged;
        [HideInInspector] [SyncVar] public float placeDistance;
        [HideInInspector] [SyncVar] public string nickName;
        [SyncVar(hook = nameof(UpdateHealth))] [HideInInspector] public int health;
        public Transform itemPosition;
        [SerializeField] private GameObject nickNameCanvas;
        private GameObject _hud;

        public void Construct(PlayerData playerData)
        {
            placeDistance = playerData.Characteristic.placeDistance;
            nickName = playerData.NickName;
            health = playerData.Characteristic.maxHealth;
        }

        public override void OnStartLocalPlayer()
        {
            _hud = AllServices.Container.Single<IUIFactory>().CreateHud(gameObject);
            //TurnOffBodyRender();
            TurnOffNickName();
        }

        private void UpdateHealth(int oldHealth, int newHealth)
        {
            OnHealthChanged?.Invoke(newHealth);
        }

        public void OnDestroy()
        {
            Destroy(_hud);
        }

        private void TurnOffBodyRender()
        {
            foreach (var component in GetComponentsInChildren<Renderer>())
            {
                component.enabled = false;
            }
        }

        private void TurnOffNickName()
        {
            nickNameCanvas.SetActive(false);
        }
    }
}