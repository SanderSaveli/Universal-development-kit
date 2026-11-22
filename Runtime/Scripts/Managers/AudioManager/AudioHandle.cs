namespace SanderSaveli.UDK
{
    public readonly struct AudioHandle
    {
        public bool IsPlaying => _pool != null && _pool.IsActive(_slotIndex, _generation);
        public bool IsCompleted => !IsPlaying;

        private readonly AudioPool _pool;
        private readonly int _slotIndex;
        private readonly ulong _generation;

        internal AudioHandle(AudioPool pool, int slotIndex, ulong generation)
        {
            _pool = pool;
            _slotIndex = slotIndex;
            _generation = generation;
        }

        public void Stop()
        {
            _pool?.StopSound(_slotIndex, _generation);
        }

        public void StopImmediate()
        {
            _pool?.StopSoundImmediate(_slotIndex, _generation);
        }
    }
}
