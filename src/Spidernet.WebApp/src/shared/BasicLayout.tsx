import React, { Component } from 'react';
import { Layout, Menu, Breadcrumb } from 'antd';
import { UserOutlined } from '@ant-design/icons';
import Home from '../components/Home';
const { SubMenu } = Menu;
const { Header, Content, Sider } = Layout;

class BasicLayout extends Component {

    state = {
      collapsed: false,
    };

    toggleCollapsed = () => {
      this.setState({
        collapsed: !this.state.collapsed,
      });
    };

    render() {
        return (
            <Layout style={{height:"100vh"}}>
                <Header className="header">
                    <div className="logo" />
                    <Menu
                        theme="dark"
                        mode="horizontal"
                        defaultSelectedKeys={['1']}
                        style={{ lineHeight: '64px' }}
                    >
                        <Menu.Item key="1" onClick={this.toggleCollapsed}>Spidernet 蜘网</Menu.Item>
                    </Menu>
                </Header>
                <Layout>
                    <Sider width={200} className="site-layout-background">
                        <Menu
                            mode="inline"
                            defaultSelectedKeys={['1']}
                            defaultOpenKeys={['BasicData']}
                            style={{ height: '100%', borderRight: 0 }}
                            inlineCollapsed={this.state.collapsed}
                        >
                            <SubMenu
                                key="BasicData"
                                title={
                                    <span>
                                        <UserOutlined />
                                        Basic data
                                    </span>
                                }
                            >
                                <Menu.Item key="Home">Home</Menu.Item>
                                <Menu.Item key="Jobs">Jobs</Menu.Item>
                                <Menu.Item key="Templates">Templates</Menu.Item>
                            </SubMenu>
                        </Menu>
                    </Sider>
                    <Layout style={{ padding: '0 24px 24px' }}>
                        <Breadcrumb style={{ margin: '16px 0' }}>
                            <Breadcrumb.Item>Job settings</Breadcrumb.Item>
                            <Breadcrumb.Item>Job</Breadcrumb.Item>
                            <Breadcrumb.Item>App</Breadcrumb.Item>
                        </Breadcrumb>
                        <Content
                            className="site-layout-background"
                            style={{
                                padding: 24,
                                margin: 0,
                                minHeight: 280,
                            }}
                        >
                            <Home />
                        </Content>
                    </Layout>
                </Layout>
            </Layout>
        );
    }
}

export default BasicLayout;