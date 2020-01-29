namespace Framework.Utility
{
    public delegate void ObservableValueChangedDelegate<TValue>(IObservableValue<TValue> observable);

    /// <summary>
    /// A value that will notify its change
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public interface IObservableValue<TValue>
    {
        /// <summary>
        /// Called when <see cref="Value"/> changes
        /// </summary>
        event ObservableValueChangedDelegate<TValue> ValueChanged;

        TValue Value { get; set; }
    }
}