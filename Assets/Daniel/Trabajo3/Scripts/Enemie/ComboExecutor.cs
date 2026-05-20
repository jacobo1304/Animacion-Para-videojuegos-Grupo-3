using System.Collections;
using UnityEngine;

namespace Clases.Clase_8.Scripts
{
    public class ComboExecutor : MonoBehaviour
    {
        public Animator animator;

        [SerializeField] private string singleTrigger = "Attack";
        [SerializeField] private string chainParam = "ControlWeight";
        [SerializeField] [Range(0f, 1f)] private float chainThreshold = 0.01f;
        [SerializeField] [Range(0f, 0.99f)] private float exit = 0.93f;
        [SerializeField] [Range(0.01f, 0.1f)] private float pulseEvery = 0.02f;
        [SerializeField] private int attackLayerIndex = 0;
        [SerializeField] private float comboStepCooldown = 1.0f;

        private bool _isRunning;
        private Coroutine _routine;
        private float _nextTriggerAt;

        public bool IsBusy() => _isRunning;

        private void Awake()
        {
            if (!animator)
            {
                animator = GetComponent<Animator>() ?? GetComponentInChildren<Animator>(includeInactive: true);
            }
        }

        public void Cancel()
        {
            if (_routine != null) StopCoroutine(_routine);
            _routine = null;
            _isRunning = false;
        }

        public void PlayCombo(ComboSequence combo)
        {
            if (_isRunning || combo == null || combo.steps == null || combo.steps.Count == 0) return;
            _routine = StartCoroutine(RunComboRoutine(combo.steps.Count));
        }

        private IEnumerator RunComboRoutine(int stepsCount)
        {
            _isRunning = true;
            try
            {
                animator.ResetTrigger(singleTrigger);
                animator.SetTrigger(singleTrigger);
                _nextTriggerAt = Time.time + comboStepCooldown;

                for (int i = 0; i < stepsCount; i++)
                {
                    bool hasNext = (i < stepsCount - 1);
                    float pulseTimer = 0f;

                    while (true)
                    {
                        AnimatorStateInfo st = animator.GetCurrentAnimatorStateInfo(attackLayerIndex);
                        float nt = st.normalizedTime % 1f;

                        if (hasNext)
                        {
                            float cw = animator.GetFloat(chainParam);
                            if (cw > chainThreshold)
                            {
                                pulseTimer += Time.deltaTime;
                                if (pulseTimer >= pulseEvery && Time.time >= _nextTriggerAt)
                                {
                                    animator.ResetTrigger(singleTrigger);
                                    animator.SetTrigger(singleTrigger);
                                    pulseTimer = 0f;
                                    _nextTriggerAt = Time.time + comboStepCooldown;
                                }
                            }
                            else
                            {
                                pulseTimer = 0f;
                            }
                        }

                        if (nt >= exit) break;
                        yield return null;
                    }
                }
            }
            finally
            {
                _isRunning = false;
                _routine = null;
            }
        }
    }
}
