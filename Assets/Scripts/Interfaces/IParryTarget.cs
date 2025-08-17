using System;

public interface IParryTarget
{
    // Gọi đúng frame vung kiếm (animation event)
    event Action OnStrike;

    // Enemy đã chết chưa (để ParrySystem tự bỏ qua)
    bool IsDead { get; }
}
