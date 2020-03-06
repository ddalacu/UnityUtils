using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Framework.Utility
{
    public static class AsyncUtils
    {
        public static async Task<Button> WaitForButtonPress([NotNull] IEnumerable<Button> toListen, CancellationToken token)
        {
            if (toListen == null)
                throw new ArgumentNullException(nameof(toListen));

            var buttons = toListen.ToArray();
            Button selected = null;

            var buttonsLength = buttons.Length;

            var delegates = new UnityAction[buttons.Length];
            for (var index = 0; index < buttonsLength; index++)
            {
                var button = buttons[index];
                if (button == null)
                    continue;
                delegates[index] = () => selected = button;
                button.onClick.AddListener(delegates[index]);
            }

            try
            { 
                loopStart:
                if (selected != null)
                    return selected;

                token.ThrowIfCancellationRequested();

                for (var index = 0; index < buttonsLength; index++)
                    if (buttons[index] != null)
                    {
                        await Task.Yield();
                        goto loopStart;
                    }

                throw new Exception("All buttons were destroyed");
            }
            finally
            {
                for (var index = 0; index < buttonsLength; index++)
                {
                    var button = buttons[index];
                    if (button == null)
                        continue;
                    button.onClick.RemoveListener(delegates[index]);
                }
            }
        }

    }
}