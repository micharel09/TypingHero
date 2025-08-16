using System;

public interface IParryTarget
{
    /// Boss bắn event này đúng tại strike-frame (đặt Animation Event gọi trong clip tấn công)
    event Action OnStrike;

    /// Parry thành công -> boss bị choáng (ParrySystem sẽ gọi)
    void Parried(float stunSeconds);
}
