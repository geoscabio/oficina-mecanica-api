# Desassocia recursos compartilhados do state histórico da API sem destruí-los.
removed {
  from = module.vpc
  lifecycle { destroy = false }
}
removed {
  from = module.eks
  lifecycle { destroy = false }
}
removed {
  from = module.rds
  lifecycle { destroy = false }
}
removed {
  from = module.ecr
  lifecycle { destroy = false }
}
