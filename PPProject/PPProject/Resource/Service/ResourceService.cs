using PPProject.Common;
using PPProject.Infrastructure.Mysql;
using PPProject.Resource.Contract;
using PPProject.Resource.Infrastructure;
using PPProject.Resource.Model;

namespace PPProject.Resource.Service
{
    public class ResourceService
    {
        private readonly UserResourceRepository _userResourceRepository;
        private readonly UserResourceHistoryRepository _userResourceHistoryRepository;
        private readonly MysqlSession _mysqlSession;

        public ResourceService(UserResourceRepository userResourceRepository, 
            UserResourceHistoryRepository userResourceHistoryRepository,
            MysqlSession mysqlSession)
        {
            _userResourceRepository = userResourceRepository;
            _userResourceHistoryRepository = userResourceHistoryRepository;
            _mysqlSession = mysqlSession;
        }

        public async Task<int> GetReourceAmount(long uId, int resourceId)
        {
            var result = await _userResourceRepository.GetAmountAsync(uId, resourceId);
            if (result == null)
            {
                var defaultAmount = DataTable.Ex.DEFAULT_CREATE_COUNT;
                result = await _userResourceRepository.CreateResource(uId, resourceId, defaultAmount);
            }
            return result.Value;
        }

        public async Task<Dictionary<int, int>> GetResourcesAmount(long uId, List<int> resourceIds)
        {
            var results = await _userResourceRepository.GetAmountsAsync(uId, resourceIds);

            return results;
        }

        public async Task<AddResourceResult> AddResourceProcess(
            long uId, 
            int resourceId, 
            int addCost,
            ResourceReasonCode reasonCode)
        {
            try
            {
                if(addCost < 0)
                    return new AddResourceResult(
                        false,
                        0,
                        AddResourceError.InavlidAddAmount);

                _mysqlSession.BeginTransaction();

                //기존의 갯수 확인
                var beforeAmount = await GetReourceAmount(uId, resourceId);

                //addCost만큼 더한 후 Max보다 크면 Fail
                var afterAmount = beforeAmount + addCost;
                var maxCount = DataTable.Ex.DEFAULT_MAX_COUNT;
                if (maxCount < afterAmount)
                    return new AddResourceResult(
                        false, 
                        0, 
                        AddResourceError.MaxCountExceed);

                //DB에 저장
                var addResult = await AddResource(uId, resourceId, addCost, maxCount);
                if(!addResult)
                    return new AddResourceResult(
                        false,
                        0,
                        AddResourceError.UpdateFailed);

                //히스토리 저장
                await _userResourceHistoryRepository.CreateHistory(
                    uId,
                    resourceId,
                    addCost,
                    beforeAmount,
                    afterAmount,
                    (int)reasonCode,
                    "Add");

                _mysqlSession.Commit();

                return new AddResourceResult(
                        true,
                        afterAmount,
                        AddResourceError.None);
            }
            catch
            {
                _mysqlSession.Rollback();
                throw;
            }
        }

        public async Task<ConsumeResourceResult> ConsumeResourceProcess(
            long uId,
            int resourceId,
            int minusCost,
            ResourceReasonCode reasonCode)
        {
            try
            {
                if (minusCost < 0)
                    return new ConsumeResourceResult(
                        false,
                        0,
                        ConsumeResourceError.InvalidConsumeCost);

                _mysqlSession.BeginTransaction();

                //기존의 갯수 확인
                var beforeAmount = await GetReourceAmount(uId, resourceId);

                //minusCost만큼 뺀 후 0보다 작으면 Fail
                var afterAmount = beforeAmount - minusCost;
                if (afterAmount < 0)
                    return new ConsumeResourceResult(
                        false,
                        0,
                        ConsumeResourceError.NotEnoughResource);

                //DB에 저장
                var consumeResult = await ConsumeResource(uId, resourceId, minusCost);
                if (!consumeResult)
                    return new ConsumeResourceResult(
                        false,
                        0,
                        ConsumeResourceError.UpdateFailed);

                //히스토리 저장
                await _userResourceHistoryRepository.CreateHistory(
                    uId,
                    resourceId,
                    -minusCost,
                    beforeAmount,
                    afterAmount,
                    (int)reasonCode,
                    "Consume");

                _mysqlSession.Commit();

                return new ConsumeResourceResult(
                        true,
                        afterAmount,
                        ConsumeResourceError.None);
            }
            catch
            {
                _mysqlSession.Rollback();
                throw;
            }
        }

        public async Task<bool> AddResource(long uId, int resourceId, int addCost, int maxCost)
        {
            var result = await _userResourceRepository.TryAddAsync(
                    uId,
                    resourceId,
                    addCost,
                    maxCost);
            return result;
        }

        public async Task<bool> ConsumeResource(long uId, int resourceId, int minusCost)
        {
            var result = await _userResourceRepository.TryConsumeAsync(
                    uId,
                    resourceId,
                    minusCost);
            return result;
        }
    }
}
