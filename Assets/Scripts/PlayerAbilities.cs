using UnityEngine;
using UnityEngine.SceneManagement;

namespace StarterAssets
{
    public class PlayerAbilities : MonoBehaviour
    {
        [Header("Abilities")]
        public float BunnyHopMultiplier = 1.2f;
        public float DashDistance = 5.0f;
        public float DashCooldown = 2.0f;
        public AudioClip jumpClip;
        public AudioClip dashClip;

        private float _lastDashTime;

        private FirstPersonController _firstPersonController;

        private void Start()
        {
            _firstPersonController = GetComponent<FirstPersonController>();
        }

        private void Update()
        {
            // Bunny Hop
            if (Input.GetKeyDown(KeyCode.Space) && _firstPersonController.Grounded)
            {
                AudioManager.instance.PlayAudio(jumpClip, 1.0f);
                _firstPersonController.SetVerticalVelocity(Mathf.Sqrt(_firstPersonController.JumpHeight * -2f * _firstPersonController.Gravity) * BunnyHopMultiplier);
            }

            // Dash
            if (Input.GetKeyDown(KeyCode.LeftShift) && Time.time - _lastDashTime > DashCooldown)
            {
                AudioManager.instance.PlayAudio(dashClip, 1.0f);
                Vector3 dashDirection = _firstPersonController.transform.forward * DashDistance;
                _firstPersonController.MoveWithVelocity(dashDirection);
                _lastDashTime = Time.time;
            }

            if(Input.GetKeyDown(KeyCode.F3))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }
}
