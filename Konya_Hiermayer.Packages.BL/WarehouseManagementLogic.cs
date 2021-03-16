using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using AutoMapper;
using Konya_Hiermayer.Packages.BL.Entities;
using Konya_Hiermayer.Packages.BL.Interfaces;
using Konya_Hiermayer.Packages.DAL.Interfaces;
using Data = Konya_Hiermayer.Packages.DAL.Entities;
using Konya_Hiermayer.Packages.BL.Entities.Validators;
using FluentValidation.Results;
using Konya_Hiermayer.Packages.BL.Entities.Exceptions;
using Microsoft.Extensions.Logging;
using Konya_Hiermayer.Packages.DAL.Entities.Exceptions;

namespace Konya_Hiermayer.Packages.BL
{
    public class WarehouseManagementLogic : IWarehouseManagementLogic
    {
        private readonly IMapper mapper;
        private readonly IWarehouseRepository wareHouseRepository;
        private readonly ILogger<WarehouseManagementLogic> logger;

        public WarehouseManagementLogic(IMapper mapper, IWarehouseRepository warehouseRepository, ILogger<WarehouseManagementLogic> logger)
        {
            this.mapper = mapper;
            this.wareHouseRepository = warehouseRepository;
            this.logger = logger;
        }

        public Warehouse ExportWarehouse()
        {
            try
            {
                logger.LogDebug("Exporting warehouse hierarchy");
                Data.Hop dataWarehouse = wareHouseRepository.Read();
                Warehouse warehouse = this.mapper.Map<Warehouse>(dataWarehouse);
                return warehouse;
            }
            catch (DataAccessLayerException e)
            {
                throw new BusinessLayerException("DAL Exception", e);
            }
        }


        public Warehouse ExportWarehouseCode(string code)
        {
            try
            {
                logger.LogDebug("Exporting warehouse hierarchy");
                Data.Hop dataWarehouse = wareHouseRepository.GetByCode(code);
                Warehouse warehouse = this.mapper.Map<Warehouse>(dataWarehouse);
                return warehouse;
            }
            catch (DataAccessLayerException e)
            {
                throw new BusinessLayerException("DAL Exception", e);
            }
        }

        public void ImportWarehouse(Warehouse warehouse)
        {
            logger.LogDebug($"Validating warehouse {warehouse.Code}");

            WarehouseValidator validator = new WarehouseValidator();
            ValidationResult result = validator.Validate(warehouse);
            Console.WriteLine(warehouse.Code);
            Console.WriteLine(warehouse.Description);
            if (result.IsValid)
            {
                Data.Warehouse dataWarehouse = this.mapper.Map<Data.Warehouse>(warehouse);
                logger.LogDebug($"Importing warehouse {warehouse.Code} into repo");
                try {
                    //TODO: delete before importing new hierarchy!!!
                    //delete Hierarchy
                    wareHouseRepository.Create(dataWarehouse);
                }
                catch (DataAccessLayerException e)
                {
                    throw new BusinessLayerException("DAL Exception", e);
                }
            }
            else
            {
                throw new InvalidWarehouseException(string.Join("\n", result.Errors));
            }
        } 
    }
}
