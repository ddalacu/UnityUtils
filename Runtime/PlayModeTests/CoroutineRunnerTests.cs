using System;
using System.Collections;
using Framework.Utility;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.TestTools;

public class CoroutineRunnerTests
{

    private IEnumerator TestSuccesfullRoutine([NotNull] Action called)
    {
        if (called == null)
            throw new ArgumentNullException(nameof(called));
        yield return null;
        called();
    }

    [UnityTest]
    public IEnumerator CoroutineIsRanSuccesfully()
    {
        bool called = false;
        CoroutineRunner.StartCoroutineStatic(TestSuccesfullRoutine(() => called = true));
        yield return null;
        yield return null;
        Debug.Assert(called);
    }

    private IEnumerator TestErrorRoutine()
    {
        yield return null;
        Debug.LogError("TestRoutineDone");
    }

    [UnityTest]
    public IEnumerator CoroutineIsStoppedSuccesfully()
    {
        Coroutine startedRoutine = CoroutineRunner.StartCoroutineStatic(TestErrorRoutine());
        CoroutineRunner.StopCoroutineStatic(startedRoutine);
        yield return null;
        yield return null;
    }
}
