using System;

public interface IParryTarget
{
    // Báo đúng frame vung kiếm + TRẢ CHÍNH ĐỐI TƯỢNG bị parry
    event Action<IParryTarget> OnStrike;

    // Enemy đã chết chưa (để ParrySystem tự bỏ qua)
    bool IsDead { get; }
}
