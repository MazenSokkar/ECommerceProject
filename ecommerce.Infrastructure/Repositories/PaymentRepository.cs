using ecommerce.Core.Entities.salah_entities;
using ecommerce.Core.IRepositories;
using ecommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ecommerce.Infrastructure.Repositories;

public class PaymentRepository(AppDbContext context) : IPaymentRepository
{
    public async Task<Payment?> FindByIdAsync(int id, CancellationToken cancellationToken = default)
        => await context.Payments
            .Include(p => p.Order)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public async Task<Payment?> FindByOrderIdAsync(int orderId, CancellationToken cancellationToken = default)
        => await context.Payments
            .Include(p => p.Order)
            .FirstOrDefaultAsync(p => p.OrderId == orderId, cancellationToken);

    public async Task<List<Payment>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
        => await context.Payments
            .Include(p => p.Order)
            .Where(p => p.Order.UserId == userId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(Payment payment, CancellationToken cancellationToken = default)
        => await context.Payments.AddAsync(payment, cancellationToken);

    public void Update(Payment payment)
        => context.Payments.Update(payment);
}