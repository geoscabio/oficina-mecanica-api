moved {
  from = module.networking
  to   = module.vpc
}

moved {
  from = module.database
  to   = module.rds
}

moved {
  from = module.registry
  to   = module.ecr
}

moved {
  from = module.kubernetes
  to   = module.eks
}
