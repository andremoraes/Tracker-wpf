using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tracker.Classes
{
    class CreateTrackerCoreTables
    {
        private List<string> Tables = new List<string> { "tracker_tb_projects", "tracker_tb_views", "tracker_tb_features","tracker_tb_vl","tracker_tb_users",
        "tracker_tb_connections","tracker_tb_stats","tracker_tb_groups_security"};
        private class Sql {
            private class Oracle
            {
                string tracker_projects = @"create table tracker_tb_projects";
                string tracker_tb_views =  @"create table tracker_tb_views
(
  pk                           RAW(16) default sys_guid() primary key ,
  prj_no                       VARCHAR2(55),
  view_parent                  VARCHAR2(25),
  swbs                         VARCHAR2(25),
  view_name                    VARCHAR2(255),
  view_enabled                 NUMBER(1) default 1,
  view_order                   NUMBER,
  long_text                    VARCHAR2(4000),
  long_sql                     CLOB,
  descr                        VARCHAR2(4000),
  view_id                      VARCHAR2(25),
  view_has_xml_columns         NUMBER default 0,
  cnn_id                       NUMBER default 1,    
  update_sql                   VARCHAR2(4000),
  xls_template                 VARCHAR2(255),
  xls_destination_namerange    VARCHAR2(255),
  xls_macro_post_data_transfer VARCHAR2(255),
  xls_exportheader             NUMBER(1) default 1,
  xls_chart_name               VARCHAR2(255),
  view_exportcode              VARCHAR2(4000),
  view_code                    VARCHAR2(4000),
  view_table_name              VARCHAR2(4000),
  --trigger based
  created                      DATE default sysdate,
  created_by                   VARCHAR2(255) default trim(replace(SYS_CONTEXT('USERENV','OS_USER') ,'WORLEYPARSONS\','')),
  modified                     DATE default sysdate,
  modified_by                  VARCHAR2(255) default trim(replace(SYS_CONTEXT('USERENV','OS_USER') ,'WORLEYPARSONS\',''))
)
";
                string tracker_tb_features = @"create table tracker_tb_features";
                string tracker_tb_stats =  @"create table tracker_tb_stats 
(usr_name VARCHAR2(255),   prj_no   VARCHAR2(55),   log_date DATE default sysdate,   log_wbs  VARCHAR2(25) not null,   log_text VARCHAR2(255))";
            }

        }
    }
}
